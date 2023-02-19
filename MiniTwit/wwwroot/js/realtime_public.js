"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/twithub").build();

connection.on("ReceiveMessage", function (message) {
    console.log(message, message.text)

    const node = document.createElement("li");
    const img = document.createElement("img");
    img.style = "width: 48px; height: 48px;"
    img.src = "https://api.dicebear.com/5.x/identicon/svg?seed=" + message.username;
    node.appendChild(img);
    const strong = document.createElement("strong");
    const a = document.createElement("a");
    a.href = "/" + message.username;
    a.textContent = message.username;
    strong.appendChild(a);
    node.appendChild(strong);
    const span = document.createElement("span");
    span.textContent = " " + message.text + " ";
    node.appendChild(span);
    const small = document.createElement("small");
    small.textContent = "— " + message.publishDate;
    node.appendChild(small);

    const list = document.getElementById("messagelist");
    list.insertBefore(node, list.children[0]);
});

connection.start().then(() => {
    connection.invoke("JoinGroup", "public").catch(function (err) {
        return console.error(err.toString());
    });
});

