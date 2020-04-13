var gulp   = require('gulp');
var concat = require('gulp-concat');
var react = require('gulp-react');
var rename = require('gulp-rename');
var uglify = require('gulp-uglify');
var browserify = require('gulp-browserify');
var cssmin = require('gulp-cssmin');

//
gulp.task('jsx',function(){
   return gulp.src(['jsx/myreact.jsx'])
      .pipe(react())
      .pipe(gulp.dest('jsx'))
   ;
});
gulp.task('browserify',['jsx'],function(){
   return gulp.src('jsx/myreact.js')
      .pipe(browserify())
      .pipe(gulp.dest('jsx'))
   ;
});

// Concat & Minify JS
gulp.task('minify', function(){
  return gulp.src('scripts/*.js')
	 .pipe(concat('all.js'))
	 .pipe(gulp.dest('dist'))
	 .pipe(rename('all.min.js'))
	 .pipe(uglify())
	 .pipe(gulp.dest('dist'))
    .on('finish', function() {
         console.log('create file success!');
     });
});
//
gulp.task('cssmin', function(){
  return gulp.src([
   'css/bootstrap-theme-pro.css',
   'css/content.css'
  ])
    .pipe(concat('all.css'))
    .pipe(gulp.dest('css_min'))
    .pipe(cssmin())
    .pipe(rename({suffix: '.min'}))
    .pipe(gulp.dest('css_min'))
    .on('finish', function() {
         console.log('create file success!');
     });
});
//cart old
gulp.task('gdtq_cart_old', function(){
  return gulp.src([
	'script_gdtq/jquery.cookie.js',
	'script_gdtq/angular.min.js',
	'script_gdtq/angular-sanitize.js',
	'script_gdtq/angular-cookies.js', 
	'script_gdtq/winter_shop_cart.js',
	'script_gdtq/winter_cart_angular.js'
  ])
	 .pipe(concat('winter_cart_all.js'))
	 .pipe(gulp.dest('dist_gdtq'))
	 .pipe(rename('winter_cart_all.min.js'))
	 .pipe(uglify())
	 .pipe(gulp.dest('dist_gdtq'))
    .on('finish', function() {
         console.log('create file success!');
     });
});
//cart new
gulp.task('gdtq_cart_new', function(){
  return gulp.src([
   'script_gdtq/jquery.cookie.js',
   'script_gdtq/angular.min.js',
   'script_gdtq/angular-sanitize.js',
   'script_gdtq/angular-cookies.js', 
   'script_gdtq/winter_shop_jquery1.js',
   'script_gdtq/winter_shop_angular1.js'
  ])
    .pipe(concat('winter_cart_new_all.js'))
    .pipe(gulp.dest('dist_gdtq'))
    .pipe(rename('winter_cart_new_all.min.js'))
    .pipe(uglify())
    .pipe(gulp.dest('dist_gdtq'))
    .on('finish', function() {
         console.log('create file success!');
     });
});

// Watch Our Files
gulp.task('watch',['minify'], function() {
  gulp.watch('scripts/*.js');
});

// Default
gulp.task('default', ['watch']);