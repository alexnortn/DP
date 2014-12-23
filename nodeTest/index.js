var server = require("./server");
var	router = require("./router");

server.begin(router.route);