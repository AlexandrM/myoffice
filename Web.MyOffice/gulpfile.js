var gulp = require('gulp');
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');
var watch = require('gulp-watch');
var eslint = require('gulp-eslint');
var child_process = require('child_process');

gulp.task('eslint', function () {
    return gulp.src(['app/**/*.js'])
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError())
        .on('error', function (error) {
            child_process.execSync('powershell -c (New-Object Media.SoundPlayer \'..\\_Tools\\error.wav\').PlaySync();', function (error, stdout, stderr) { });
        });
});

gulp.task('Release', ['eslint'], function () {
    child_process.execSync('powershell -c (New-Object Media.SoundPlayer \'..\\_Tools\\success.wav\').PlaySync();', function (error, stdout, stderr) { });
    return gulp.src('app/**/*.js')
        .pipe(concat('dis.js'))
        .pipe(uglify({
            beautify: true,
            mangle: true
        }))
        .pipe(gulp.dest('scripts'));
});

gulp.task('Debug', ['eslint'], function () {
    child_process.execSync('powershell -c (New-Object Media.SoundPlayer \'..\\_Tools\\success.wav\').PlaySync();', function (error, stdout, stderr) { });
    gulp.src('app/**/*.js')
        .pipe(concat('dis.js'))
        .pipe(gulp.dest('scripts'));
});

gulp.task('watch', function () {
    gulp.run(['Debug']);
    gulp.watch('app/**/*.js', ['Debug']);
});