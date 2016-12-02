var gulp = require("gulp");
var uglify = require("gulp-uglify");
var concat = require("gulp-concat");
var watch = require('gulp-watch');
var gutil = require('gulp-util');
var batch = require('gulp-batch');
var jsValidate = require('gulp-jsvalidate');
var eslint = require('gulp-eslint');

gulp.task('eslint', function () {
    return gulp.src(['app/**/*.js'])
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError());
});

gulp.task("combine-and-uglify", ['eslint'], function () {
    return gulp.src('app/*.js')
        .pipe(concat('dis.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('scripts'));
});

gulp.task("combine", ['eslint'], function () {

    gulp.src('app/**/*.js')
        .pipe(concat('dis.js'))
        .pipe(gulp.dest('scripts'));
});

gulp.task("watch", function () {
    gulp.run(["combine"]);
    gulp.watch('app/**/*.js', ["combine"]);
});