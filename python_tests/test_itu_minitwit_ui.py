import psycopg2
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.firefox.service import Service
from selenium.webdriver.firefox.options import Options


GUI_URL = "http://localhost:8765"
DB_URL = "postgres://localhost:2345"

try:
    conn = psycopg2.connect("dbname='minitwitdb' user='postgres' host='localhost' password='postgres' port='2345'")
except:
    print("I am unable to connect to the database")

cur = conn.cursor()

def _get_user_by_name(name):
    cur.execute(f"""SELECT * FROM "Users" WHERE "Username" = '{name}';""")
    user = cur.fetchall()
    return None if user == [] else user[0]

def _delete_user_by_name(name):
    cur.execute(f"""DELETE FROM "Users" WHERE "Username" = '{name}';""")
    conn.commit()

def _get_user_tweets_by_name(name):
    user = _get_user_by_name(name)
    cur.execute(f"""SELECT "Text" FROM "Messages" WHERE "AuthorId" = {user[0]};""")
    tweets = cur.fetchall()
    return map(lambda m : m[0], tweets)

def _get_follows_by_name(name):
    user = _get_user_by_name(name)
    cur.execute(
        f"""
        SELECT u."Username"
        FROM "UserUser" f
        JOIN "Users" u ON u."Id" = f."FollowsId"
        WHERE f."FollowersId" = {user[0]};
        """)
    follows = cur.fetchall()
    return map(lambda f : f[0], follows)


# seed with "You" user
if (_get_user_by_name('You') == None):
    cur.execute("""INSERT INTO "Users" VALUES (-1, 'You', 'You@mail.com', 'PASSWORD', 'HASH')""")
    cur.execute("""INSERT INTO "Messages" VALUES (-1, -1, 'Read this', '2023-03-01', 0)""")
    cur.execute("""INSERT INTO "Messages" VALUES (-2, -1, 'I am You', '2023-03-02', 0)""")
    conn.commit()

def _register_user_via_gui(driver, data):
    driver.get(f"{GUI_URL}/register")

    wait = WebDriverWait(driver, 5)
    buttons = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "actions")))
    input_fields = driver.find_elements(By.TAG_NAME, "input")

    for idx, str_content in enumerate(data):
        input_fields[idx].send_keys(str_content)
    input_fields[4].send_keys(Keys.RETURN)

    wait = WebDriverWait(driver, 5)
    flashes = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "flashes")))

    return flashes

def _login_user_via_gui(driver, data):
    driver.get(f"{GUI_URL}/login")

    wait = WebDriverWait(driver, 5)
    buttons = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "actions")))
    input_fields = driver.find_elements(By.TAG_NAME, "input")

    for idx, str_content in enumerate(data):
        input_fields[idx].send_keys(str_content)
    input_fields[2].send_keys(Keys.RETURN)

    wait = WebDriverWait(driver, 5)
    flashes = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "flashes")))

    return flashes

def _logout_user_via_gui(driver):
    wait = WebDriverWait(driver, 5)
    driver.find_element(By.PARTIAL_LINK_TEXT, "sign out").click()

    wait = WebDriverWait(driver, 5)
    flashes = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "flashes")))

    return flashes

def _tweet_via_gui(driver, data):
    driver.get(f"{GUI_URL}/")

    wait = WebDriverWait(driver, 5)
    buttons = wait.until(EC.presence_of_all_elements_located((By.ID, "Text")))
    input_fields = driver.find_elements(By.TAG_NAME, "input")

    for idx, str_content in enumerate(data):
        input_fields[idx].send_keys(str_content)
    input_fields[1].send_keys(Keys.RETURN)

    wait = WebDriverWait(driver, 5)
    flashes = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "flashes")))

    return flashes

def _follow_via_gui(driver, data):
    driver.get(f"{GUI_URL}/{data[0]}")

    wait = WebDriverWait(driver, 5)
    buttons = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "link-button")))
    driver.find_element(By.TAG_NAME, "button").click()

    wait = WebDriverWait(driver, 5)
    flashes = wait.until(EC.presence_of_all_elements_located((By.CLASS_NAME, "flashes")))

    return flashes

def test_register_user_via_gui():
    """
    This is a UI test. It only interacts with the UI that is rendered in the browser and checks that visual
    responses that users observe are displayed.
    """
    firefox_options = Options()
    firefox_options.add_argument("--headless")
    # firefox_options = None

    with webdriver.Firefox(service=Service("./geckodriver"), options=firefox_options) as driver:
        generated_msg = _register_user_via_gui(driver, ["Me", "me@some.where", "secure123", "secure123"])[0].text
        expected_msg = "You were successfully registered and can login now."
        assert generated_msg == expected_msg

    # cleanup, make test case idempotent
    _delete_user_by_name('Me')


def test_register_user_via_gui_and_check_db_entry():
    """
    This is an end-to-end test. Before registering a user via the UI, it checks that no such user exists in the
    database yet. After registering a user, it checks that the respective user appears in the database.
    """
    firefox_options = Options()
    firefox_options.add_argument("--headless")
    # firefox_options = None

    with webdriver.Firefox(service=Service("./geckodriver"), options=firefox_options) as driver:

        assert _get_user_by_name('Me') == None

        generated_msg = _register_user_via_gui(driver, ["Me", "me@some.where", "secure123", "secure123"])[0].text
        expected_msg = "You were successfully registered and can login now."
        assert generated_msg == expected_msg

        assert _get_user_by_name("Me")[1] == "Me"

        # cleanup, make test case idempotent
        _delete_user_by_name('Me')

def test_register_then_login():
    firefox_options = Options()
    firefox_options.add_argument("--headless")
    # firefox_options = None
    with webdriver.Firefox(service=Service("./geckodriver"), options=firefox_options) as driver:
        assert _get_user_by_name('Me') == None

        _register_user_via_gui(driver, ["Me", "me@some.where", "secure123", "secure123"])
        generated_msg = _login_user_via_gui(driver, ["Me", "secure123"])[0].text
        expected_msg = "You were logged in."

        assert generated_msg == expected_msg

        _delete_user_by_name('Me')

def test_register_login_then_logout():
    firefox_options = Options()
    firefox_options.add_argument("--headless")
    # firefox_options = None
    with webdriver.Firefox(service=Service("./geckodriver"), options=firefox_options) as driver:
        assert _get_user_by_name('Me') == None

        _register_user_via_gui(driver, ["Me", "me@some.where", "secure123", "secure123"])
        _login_user_via_gui(driver, ["Me", "secure123"])
        generated_msg = _logout_user_via_gui(driver)[0].text
        expected_msg = "You were logged out."

        assert generated_msg == expected_msg

        _delete_user_by_name('Me')

def test_register_login_then_tweet():
    firefox_options = Options()
    firefox_options.add_argument("--headless")
    # firefox_options = None
    with webdriver.Firefox(service=Service("./geckodriver"), options=firefox_options) as driver:
        assert _get_user_by_name('Me') == None

        _register_user_via_gui(driver, ["Me", "me@some.where", "secure123", "secure123"])
        _login_user_via_gui(driver, ["Me", "secure123"])
        generated_msg = _tweet_via_gui(driver, ["Hello"])[0].text
        expected_msg = "Your message was recorded."
        
        assert generated_msg == expected_msg
        
        generated_msg = _tweet_via_gui(driver, ["My password is very secure"])[0].text
        generated_msg = _tweet_via_gui(driver, ["123"])[0].text

        msgs = _get_user_tweets_by_name('Me')

        assert "Hello" in msgs
        assert "My password is very secure" in msgs
        assert "123" in msgs

        _delete_user_by_name('Me')

def test_register_login_then_follow():
    firefox_options = Options()
    firefox_options.add_argument("--headless")
    # firefox_options = None
    with webdriver.Firefox(service=Service("./geckodriver"), options=firefox_options) as driver:
        assert _get_user_by_name('Me') == None

        _register_user_via_gui(driver, ["Me", "me@some.where", "secure123", "secure123"])
        _login_user_via_gui(driver, ["Me", "secure123"])
        generated_msg = _follow_via_gui(driver, ["You"])[0].text
        expected_msg = "You are now following You."
        
        assert generated_msg == expected_msg

        fllws = _get_follows_by_name('Me')

        assert "You" in fllws

        driver.get(f"{GUI_URL}/")
        wait = WebDriverWait(driver, 5)
        msgs = wait.until(EC.presence_of_all_elements_located((By.TAG_NAME, "li")))
        elements = driver.find_elements(By.TAG_NAME, "li")
        msgs = list(map(lambda m : m.text, elements))

        assert len([m for m in msgs if "Read this" in m]) == 1
        assert len([m for m in msgs if "I am You" in m]) == 1

        _delete_user_by_name('Me')