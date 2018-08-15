var gulp            = require('gulp'),
    scss            = require('gulp-sass'),
    browserSync     = require('browser-sync'),
    autoprefixer    = require('gulp-autoprefixer'),
    livereload      = require('gulp-livereload');


/*TASK*/
gulp.task('scss', function () {
    return gulp.src('scss/common.scss')
        .pipe(scss())
        // .pipe(autoprefixer({
        //     browsers: ['last 5 versions'],
        //     cascade: false
        // }))
        .pipe(gulp.dest('css'))
        .pipe(browserSync.reload({stream: true}))
        .pipe(livereload());
});

gulp.task('browser-sync', function(){
    browserSync({
        server: {
            baseDir: ''
        },
        notify: false
    })
});

gulp.task('watch',['browser-sync', 'scss'], function(){
    livereload.listen();
    gulp.watch('scss/*.scss', ['scss'], browserSync.reload);
});



// var gulp = require('gulp');
// var browserSync = require('browser-sync');
// var sass = require('gulp-sass');
// var autoprefixer = require('gulp-autoprefixer');
//
//
// // compile styles
// gulp.task('styles', function() {
//     gulp.src(['scss/common.scss'])
//         .pipe(sass())
//         .pipe(autoprefixer())
//         .pipe(gulp.dest('css/'))
// });
//
// gulp.task('default', function(){
//     gulp.watch("scss/**/*.scss", ['styles']);
// });
