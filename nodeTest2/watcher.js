/*

	NodeJs starter program
	Alex Norton
	2015

*/


const 
	fs = require('fs'),
	filename = process.argv[2];
if (!filename) {
	throw Error("A file to watch must be specified!");
}
fs.watch('target.txt', function() {
	console.log("File "+ filename + " target.txt just changed!");
});
console.log("Now watching " + filename + " target.txt for changes...");
