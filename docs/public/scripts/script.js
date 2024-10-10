lightbox.option({
  'fadeDuration': 300,
  'imageFadeDuration': 300,
  'resizeDuration': 200
});

function fadeAnime(){
  $('.vp-doc > div > *').each(function(){
    var elemPos = $(this).offset().top;
    var scroll = $(window).scrollTop();
    var windowHeight = $(window).height();
    if (scroll >= elemPos - windowHeight - 50){
      $(this).addClass('fadeUp');
    }
  });
}

function countOl(){
  $("ol").each(function() {
    $(this).css({
      'counterReset': 'cnt ' + ($(this).attr("start") - 1)
    });
  });
};

function init(){
  fadeAnime();
  countOl();
};

$(window).on('scroll', fadeAnime);
$(window).on('load', init);

var config = { attributes: true };

var modoc = new MutationObserver(init);
var mo = new MutationObserver(function() {
  init();
  modoc.observe(document.querySelector('.vp-doc'), config);
});

var vpdoc = document.querySelector('.vp-doc');
if(vpdoc) modoc.observe(vpdoc, config);
mo.observe(document.querySelector('.VPContent'), config);