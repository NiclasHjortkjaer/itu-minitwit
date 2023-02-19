"use strict";

realtimeConnection.start().then(() => {
    realtimeConnection.invoke("JoinGroup", window.location.pathname.replace("/", "").toLowerCase())
        .catch(function (err) {
            return console.error(err.toString());
    });
});