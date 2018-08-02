var notify = require("gulp-notify");
var gutil = require('gulp-util');

module.exports = function() {

    var args = Array.prototype.slice.call(arguments);

    console.log(args);

    // Send error to notification center with gulp-notify
    notify.onError({
        title: "Compilation Error",
        message: "<%= error.message %>"
    }).apply(this, args);

    gutil.beep();

    // Keep gulp from hanging on this task
    this.emit('end');
};