/**
 * Assign plugin dependencies
 */
var gulp = require('gulp');
var sass = require('gulp-ruby-sass');
var autoprefix = require('gulp-autoprefixer');
var uglify = require('gulp-uglify');
var browserify = require('browserify');
var source = require('vinyl-source-stream');
var buffer = require('vinyl-buffer');
var handleErrors = require('./js/utils/handleErrors');
var gutil = require('gulp-util');
var ngAnnotate = require('gulp-ng-annotate');
var size = require('gulp-size');

/**
 * Compile SASS files
 */
gulp.task('sass', function () {
    var sassConfig = gutil.env.production ? {style: 'compressed'} : {style: 'expanded'};
    sassConfig['sourcemap=none'] = true;
    return sass('./sass/', sassConfig)
        .on('error', handleErrors)
        .pipe(autoprefix({
            browsers: ['last 10 version'],
            cascade: false
        }))
        .on('error', handleErrors)
        .pipe(size({
            showFiles: true,
            title: 'css'
        }))
        .pipe(gulp.dest('./public/css'));
});

/**
 * Minimize "framework" JavaScript
 */
gulp.task('js', function () {
    return browserify('./js/index.js')
        .bundle()
        .on('error', handleErrors)
        .pipe(source('app.min.js'))
        .pipe(buffer())
        .pipe(gutil.env.production ? ngAnnotate() : gutil.noop())
        .pipe(gutil.env.production ? uglify({
            compress: {
                negate_iife: false
            },
            preserveComments: 'some'
        }) : gutil.noop())
        .pipe(size({
            title: 'js'
        }))
        .pipe(gulp.dest('./public/js/build'));
});

/**
 * File watcher
 */
gulp.task('watch', function () {
    gulp.watch('sass/**/*.scss', ['sass']).on('change', logTime);
    gulp.watch(['js/**/*.js', '!js/utils/*.js'], ['js']).on('change', logTime);
});

/**
 * Default task
 */
gulp.task('default', ['sass', 'js', 'watch']);

/**
 * Utility functions
 */
function logTime(event) {
    console.log('[' + getTheTime() + '] ' + 'File ' + event.path + ' was ' + event.type + ', running tasks...');
}

function getTheTime() {
    var now = new Date();
    return ((now.getHours() < 10) ? "0" : "") + now.getHours() + ":" + ((now.getMinutes() < 10) ? "0" : "") + now.getMinutes() + ":" + ((now.getSeconds() < 10) ? "0" : "") + now.getSeconds();
}
