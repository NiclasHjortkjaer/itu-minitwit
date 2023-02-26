# -*- coding: utf-8 -*-
"""
    MiniTwit Tests
    ~~~~~~~~~~~~~~

    Tests the MiniTwit application.

    :copyright: (c) 2010 by Armin Ronacher.
    :license: BSD, see LICENSE for more details.
"""
import unittest

from requests import Session
from urllib.parse import urljoin

class LiveServerSession(Session):
    def __init__(self, base_url=None):
        super().__init__()
        self.base_url = base_url

    def request(self, method, url, *args, **kwargs):
        joined_url = urljoin(self.base_url, url)
        return super().request(method, joined_url, *args, **kwargs)

class MiniTwitTestCase(unittest.TestCase):

    def setUp(self):
        """Before each test, set up a blank database"""
        self.app = LiveServerSession("http://localhost:8765")

    def register(self, username, password, password2=None, email=None):
        """Helper function to register a user"""
        if password2 is None:
            password2 = password
        if email is None:
            email = username + '@example.com'
        return self.app.post('/register', data={
            'username': username,
            'password': password,
            'passwordRepeat': password2,
            'email': email,
        }, allow_redirects=True)

    def login(self, username, password):
        """Helper function to login"""
        return self.app.post('/login', data={
            'username': username,
            'password': password
        }, allow_redirects=True)

    def register_and_login(self, username, password):
        """Registers and logs in in one go"""
        self.register(username, password)
        return self.login(username, password)

    def logout(self):
        """Helper function to logout"""
        return self.app.get('/logout', allow_redirects=True)

    def add_message(self, text):
        """Records a message"""
        rv = self.app.post('/', data={'text': text},
                           allow_redirects=True)
        if text:
            assert 'Your message was recorded' in rv.text
        return rv

    # testing functions

    def test_register(self):
        """Make sure registering works"""
        rv = self.register('register_user1', 'default')
        assert 'You were successfully registered ' \
               'and can login now' in rv.text
        rv = self.register('register_user1', 'default')
        assert 'The username is already taken' in rv.text
        rv = self.register('', 'default')
        assert 'You have to enter a username' in rv.text
        rv = self.register('meh', '')
        assert 'You have to enter a password' in rv.text
        rv = self.register('meh', 'x', 'y')
        assert 'The two passwords do not match' in rv.text
        rv = self.register('meh', 'foo', email='broken')
        assert 'You have to enter a valid email address' in rv.text

    def test_login_logout(self):
        """Make sure logging in and logging out works"""
        rv = self.register_and_login('login_logout_user1', 'default')
        assert 'You were logged in' in rv.text
        rv = self.logout()
        assert 'You were logged out' in rv.text
        rv = self.login('login_logout_user1', 'wrongpassword')
        assert 'Invalid password' in rv.text
        rv = self.login('login_logout_user2', 'wrongpassword')
        assert 'Invalid username' in rv.text

    def test_message_recording(self):
        """Check if adding messages works"""
        self.register_and_login('message_foo', 'default')
        self.add_message('test message 1')
        self.add_message('<test message 2>')
        rv = self.app.get('/')
        assert 'test message 1' in rv.text
        assert '&lt;test message 2&gt;' in rv.text

    def test_timelines(self):
        """Make sure that timelines work"""
        self.register_and_login('timeline_foo', 'default')
        self.add_message('the message by timeline_foo')
        self.logout()
        self.register_and_login('timeline_bar', 'default')
        self.add_message('the message by timeline_bar')
        rv = self.app.get('/public')
        assert 'the message by timeline_foo' in rv.text
        assert 'the message by timeline_bar' in rv.text

        # timeline_bar's timeline should just show timeline_bar's message
        rv = self.app.get('/')
        assert 'the message by timeline_foo' not in rv.text
        assert 'the message by timeline_bar' in rv.text

        # now let's follow timeline_foo
        rv = self.app.post('/timeline_foo', allow_redirects=True)
        assert 'You are now following timeline_foo' in rv.text

        # we should now see timeline_foo's message
        rv = self.app.get('/')
        assert 'the message by timeline_foo' in rv.text
        assert 'the message by timeline_bar' in rv.text

        # but on the user's page we only want the user's message
        rv = self.app.get('/timeline_bar')
        assert 'the message by timeline_foo' not in rv.text
        assert 'the message by timeline_bar' in rv.text
        rv = self.app.get('/timeline_foo')
        assert 'the message by timeline_foo' in rv.text
        assert 'the message by timeline_bar' not in rv.text

        # now unfollow and check if that worked
        rv = self.app.post('/timeline_foo', allow_redirects=True)
        assert 'You are no longer following timeline_foo' in rv.text
        rv = self.app.get('/')
        assert 'the message by timeline_foo' not in rv.text
        assert 'the message by timeline_bar' in rv.text

if __name__ == '__main__':
    unittest.main()