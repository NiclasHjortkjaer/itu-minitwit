"use strict";

realtimeConnection.start().then(() => {
    realtimeConnection.invoke("JoinGroup", "public").catch(function (err) {
        return console.error(err.toString());
    });
});

