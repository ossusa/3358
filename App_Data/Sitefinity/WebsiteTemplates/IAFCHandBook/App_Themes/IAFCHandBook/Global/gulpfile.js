var gulp            = require('gulp'),
    scss            = require('gulp-sass'),
    browserSync     = require('browser-sync'),
    autoprefixer    = require('gulp-autoprefixer'),
    livereload      = require('gulp-livereload');


/*TASK*/
gulp.task('scss', function () {
    return gulp.src('scss/resourcedetails.scss')
        .pipe(scss())
        .pipe(autoprefixer({
            browsers: ['last 5 versions'],
            cascade: false
        }))
        .pipe(gulp.dest(''))
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