var gulp = require("gulp");
var uglify = require("gulp-uglify");
var concat = require("gulp-concat");
var watch = require('gulp-watch');
var eslint = require('gulp-eslint');
var child_process = require('child_process');

gulp.task('eslint', function () {
    return gulp.src(['app/**/*.js'])
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError())
        .on('error', (error) => {
            child_process.execSync('powershell -c (New-Object Media.SoundPlayer \'..\\_Tools\\error.wav\').PlaySync();', (error, stdout, stderr) => { });
        });
});

gulp.task("combine-and-uglify", ['eslint'], function () {
    child_process.execSync('powershell -c (New-Object Media.SoundPlayer \'..\\_Tools\\success.wav\').PlaySync();', (error, stdout, stderr) => { });
    return gulp.src('app/*.js')
        .pipe(concat('dis.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('scripts'));
});

gulp.task("combine", ['eslint'], function () {
    child_process.execSync('powershell -c (New-Object Media.SoundPlayer \'..\\_Tools\\success.wav\').PlaySync();', (error, stdout, stderr) => { });
    gulp.src('app/**/*.js')
        .pipe(concat('dis.js'))
        .pipe(gulp.dest('scripts'));
});

gulp.task("watch", function () {
    gulp.run(["combine"]);
    gulp.watch('app/**/*.js', ["combine"]);
});