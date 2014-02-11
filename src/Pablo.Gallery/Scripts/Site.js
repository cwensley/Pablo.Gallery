
$(document).ready(function () {

	$(document).bind('cbox_open', function () {
		$('html').css({ overflow: 'hidden' });
	}).bind('cbox_closed', function () {
		$('html').css({ overflow: 'auto' });
	});
});

(function( $ ){
	var defaults = {
		images: null,
		rel: null,
		title: null,
		loadMore: null,
		loadCloseness: 5
	};

	var methods = {
		init : function(options) {
			var o = $.extend({ }, defaults, options);

			var items = $(o.images);
			$(window).resize(function() {
				$.colorbox.resize({
					width: $(window).width() > 800 ? $(window).width() * 0.8 : $(window).width(),
					height:$(window).height()
				});
			});

			var cboptions = {
					rel: o.rel,
					photo: function() {
						return $(this).data('type') == 'image';
					},
					iframe: function() {
						return $(this).data('type') != 'image';
					},
					fixed: true,
					reposition: false,
					loop: false,
					height: function() { return $(window).height(); },
					maxWidth: function() { return $(window).width(); },
					scalePhotos: false,
					width: function() { return $(window).width() > 640 ? $(window).width() * 0.8 : $(window).width(); },
					initialHeight: Math.min(480, $(window).width()), 
					initialWidth: Math.min(640, $(window).height()),
					href: function() { return $(this).data('img'); },
					title: function() {
						var url = $(this).attr('href');
						var ret = '<a href="' + url + '" target="_blank">Open In New Window</a>';
						url = $(this).data('download');
						ret += ' <a href="' + url + '">Download</a>';
						var name = $(this).find('.file-name').html();
						ret += ' <span class="cb-name">' + name + '</span>';
						if (o.title)
							ret += o.title($(this));
						return ret;
					},
					onComplete: function() {
						if (o.loadMore)
						{
							var indexes = /(\d+)\D+(\d+)/i.exec($('#cboxCurrent').html())
							var current = parseInt(indexes[1]);
							var total = parseInt(indexes[2]);
							if (current > total - o.loadCloseness)
								o.loadMore(function() {
									$(o.images).colorbox($.extend({}, cboptions));
									items = $(o.images);
								});
						}

					}
				};

			$(this).on('click', o.images, function(event) {
				event.preventDefault();
				$(o.images).colorbox($.extend({ open: true }, cboptions));
			});
			return this;
		},
	};

	$.fn.gallery = function(methodOrOptions) {
		if ( methods[methodOrOptions] ) {
			return methods[ methodOrOptions ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof methodOrOptions === 'object' || ! methodOrOptions ) {
			// Default to "init"
			return methods.init.apply( this, arguments );
		} else {
			$.error( 'Method ' +  methodOrOptions + ' does not exist on jQuery.gallery' );
		}	
	};

})( jQuery );

(function( $ ){

	var defaults = {
		type: "get",
		template: null,
		url: null,
		error: null,
		params: { },
		scrollable: $(window),
		content: $(document),
		loading: null,
		selector: null,
		closeness: $(window).height()
	};

	var o;

	var functions = {
		  	load: function(loaded) {
		  		var url = o.url;
		  		var data = null;
		  		if (o.type == "get")
		  			url = url + "?" + $.param(o.params);
		  		else
					data = JSON.stringify(o.params);

				$.ajax({
					type: o.type,
					url: url,
					async: true,
					data: data,
					contentType: "application/json",
					success: function (data) {
						if (o.selector != null)
							data = o.selector(data);
						if (data != null && data.length) {
							var template = $.templates(o.template);
							o.result.append(template.render(data));
							o.params.Page++;
						}
						else
							o.finished = true;
						$(o.loading).hide();
						o.inProgress = false;
						if (loaded)
							loaded();
					},
					error: o.error
				});

		  	}
		};

	var methods = {
		init : function(options) {
	  
			o = $.extend(defaults, options);
			o.result = $(this);
			o.finished = false;
			o.scrollable = $(o.scrollable);
			o.content = $(o.content);
			o.inProgress = true;
			if (!o.params.Page)
				o.params.Page = 0;
			// do our initial load
			functions.load();

			// when we reach close to the bottom of the screen, reload
			o.scrollable.scroll(function () {
				if (!o.finished && !o.inProgress && o.scrollable.scrollTop() > o.content.height() - o.scrollable.height() - o.closeness) {
					methods.load();
				}
			});

			return this;
		},
		load: function(loaded) {
			if (!o.finished && !o.inProgress) {
				o.inProgress = true;
				$(o.loading).show();
				functions.load(loaded);
			}
		}
	};

	$.fn.pageloader = function(methodOrOptions) {
		if ( methods[methodOrOptions] ) {
			return methods[ methodOrOptions ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof methodOrOptions === 'object' || ! methodOrOptions ) {
			// Default to "init"
			return methods.init.apply( this, arguments );
		} else {
			$.error( 'Method ' +  methodOrOptions + ' does not exist on jQuery.gallery' );
		}	
	};

})( jQuery );
