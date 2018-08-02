/**
 * Main JavaScript file that pulls in all dependencies and concatenates into a single concatenated file
 */
var jquery = require("jquery");
require('slick-carousel');
var branding = require('./branding')(jquery);
var rss = require('./rsg-rss')(jquery);
var gallery = require('./gallery')(jquery);
var groupslick = require('./group-slick')(jquery);
var copy = require('./copy')(jquery);
var nav = require('./nav')(jquery);
var forms = require('./forms')(jquery);
var search = require('./search')(jquery);
var carousel = require('./carousel')(jquery);
var resourcelib = require('./resourcelib')(jquery);
var slick = require('./slick')(jquery);
var print = require('./utility')(jquery);
//var fb = require('./facebook-share')(jquery);
var tw = require('./twitter-share')(jquery);
var featured = require('./featured-hide')(jquery);
var categories = require('./categories')(jquery);
var hide = require('./hide-authors')(jquery);
var careerrss = require('./career-feed')(jquery);
var pdf = require('./pdf-window')(jquery);
require('./calendar')();
