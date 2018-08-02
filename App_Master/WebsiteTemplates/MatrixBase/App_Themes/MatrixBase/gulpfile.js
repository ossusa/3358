var gulp = require('gulp');
var gutil = require('gulp-util');
var sass = require('gulp-sass');
var uglify = require("gulp-uglify");
var autoprefix = require('gulp-autoprefixer');
var browserify = require('browserify');
var source = require('vinyl-source-stream');
var buffer = require('vinyl-buffer');
var gulpSize = require('gulp-size');

/**
 * Tasks which will run on 'gulp' command 
 * running 'gulp' in the command line will result in unminified CSS/JavaScript files
 * running 'gulp --production' will result in minified CSS/JavaScript files
 **/
gulp.task('default', ['sass', 'js', 'watch']);

/**
 * Compile Sass
 **/
gulp.task('sass', function () {
    var sassConfig = gutil.env.production ? {outputStyle: 'compressed'} : {outputStyle: 'expanded'};
    
    gulp.src('./sass/**/*.scss')
        .pipe(sass(sassConfig).on('error', sass.logError))
        .pipe(autoprefix({
                browsers: ['last 10 version'],
                cascade: false
            }))
        .pipe(gulpSize({
            showFiles: true,
            title: 'css'
        }))
        .pipe(gulp.dest('./Global'));

    return;
});

/**
 * Build front end Js files
 */
gulp.task('js', function() {

    return browserify('./js/main.js')
        .transform("babelify", {presets: ["es2015"]})
        .bundle()
        .on('error', onError)
        .pipe(source( 'main.min.js'))
        .pipe(buffer())
        .pipe(gutil.env.production ? uglify() : gutil.noop())
        .pipe(gulpSize({
            showFiles: true,
            title: 'js'
        }))
        .pipe(gulp.dest('./js/min'));
        
});

/**
 * File watcher
 */
gulp.task('watch', function () {
    gulp.watch('sass/**/*.scss', ['sass']).on('change', logTime);
    gulp.watch(['js/**/*.js', '!js/min/**/*.js'], ['js']).on('change', logTime);
});

/**
 * Utility functions
 */
function onError(err) {
    gutil.beep();
    console.log('There was an error: ' + err.message);
    this.emit('end'); // prevent watch from hanging on error
}

function logTime(event) {
    var now = getTheTime();
    console.log('[' + now + '] ' + 'File ' + event.path + ' was ' + event.type + ', running tasks...');
}

function getTheTime() {
    var now = new Date();
    return ((now.getHours() < 10) ? "0" : "") + now.getHours() + ":" + ((now.getMinutes() < 10) ? "0" : "") + now.getMinutes() + ":" + ((now.getSeconds() < 10) ? "0" : "") + now.getSeconds();
}
