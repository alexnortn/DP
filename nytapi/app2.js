var $search;
var $progress;

var API = "0ea1d3507c21104ebad816c715af2e3b:6:70277900";
var START_YEAR = 1900;
var END_YEAR = 2015;
//used to get progress (not done yet)
var totalDone = 0;

$(document).ready(function() {
	
	$("#searchButton").on("click", search);
	$search = $("#search");
	$progress = $("#progress");
});

function fetchForYear(year, term) {
	//YYYYMMDD
	var startYearStr = year + "0101";
	var endYearStr = year + "1231";
	console.log('doing year '+year);
	
	return $.get("http://api.nytimes.com/svc/search/v2/articlesearch.json", {
		"api-key":API,
		sort:"oldest",
		begin_date:startYearStr,
		end_date:endYearStr,
		fq:"headline:(\""+term+"\")",
		fl:"headline,snippet,multimedia,pub_date"}, function(res) {
			//Ok, currently assume a good response
			//todo - check the response
			//console.dir(res.response);
			totalDone++;
	}, "JSON");
	
}

/*
Given an array of data, I process X items async at a time.
When done, I see if I need to do more, and if so, I call it in
Y miliseconds. The idea being I do chunks of aysnc requests with
a 'pad' between them to slow down the requests.
*/
var globalData;
var searchTerm;
var currentYear;
var PER_SET = 10;
function processSets() {
	var promises = [];
	for(var i=0;i<PER_SET;i++) {
		var yearToGet = currentYear + i;
		if(yearToGet <= END_YEAR) {
			promises.push(fetchForYear(yearToGet,searchTerm));
		}
	}
	$.when.apply($, promises).done(function() {
		console.log('DONE with Set '+promises.length);
		
		//update progress
		var percentage = Math.floor(totalDone/(END_YEAR-START_YEAR)*100);
		$progress.text("Working on data, "+percentage +"% done.");
		
		//massage into something simpler
		// handle cases where promises array is 1
		if(promises.length === 1) {
			var toAddRaw = arguments[0];
			globalData.push({
				year:currentYear,
				results:toAddRaw.response.meta.hits
			});			
		} else {
			for(var i=0,len=arguments.length;i<len;i++) {
				var toAddRaw = arguments[i][0];
				var year = currentYear+i;
				globalData.push({
					year:year,
					results:toAddRaw.response.meta.hits
				});
			}
		}
		currentYear += PER_SET;

		//Am I done yet?
		if(currentYear <= END_YEAR) {
			setTimeout(processSets, 900);
		} else {
			$progress.text("");
			render(globalData);	
		}
	});

}

function search() {
	var term = $search.val();
	if(term === '') return;
	totalDone = 0;
	
	console.log("Searching for "+term);

	globalData = [];
	searchTerm = term;
	currentYear = START_YEAR;

	$progress.text("Beginning work...");
	processSets();
}
	
function render(searchData) {
	console.dir(searchData);

	var chartData = [];
	for(var i=0, len=searchData.length; i<len; i++) {
		chartData.push({name:searchData[i].year,y:searchData[i].results});	
	}
	console.dir(chartData);
	$('#container').highcharts({
        chart: {
            type: 'line'
        },
        title: {
            text: 'Occurrences of '+searchTerm+ ' in NYT Headlines'
        },
        xAxis: {
			type:'datetime'
        },
        yAxis: {
            title: {
                text: 'Occurrences'
            },
			min:0
        },
		legend: {
			enabled:false
		},
		tooltip: {
			formatter:function() {
				return '<b>'+this.y+' matches in '+this.point.name+'</b>';	
			}
		},
        series: [{
            data: chartData,
			pointStart:Date.UTC(START_YEAR, 0, 1),
			pointInterval:365*24*3600*1000
        }]
    });
	
}