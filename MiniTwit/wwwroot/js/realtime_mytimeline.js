"use strict";

realtimeConnection.start().then(() => {
    realtimeConnection.invoke("JoinGroup", "mytimeline").catch(function (err) {
        return console.error(err.toString());
    });
});

