(function( $ ){

	var o = { };

	var defaults = {
		template: '<ul class="pablodraw-gallery">\
{{for files}}\
	<li class="pablodraw-gallery-file">\
		<a href="{{attr:~root.baseUrl}}{{attr:displayUrl}}" title="{{attr:fileName}}" data-type="{{attr:displayType}}" data-url="{{attr:~root.baseUrl}}{{attr:url}}">\
			<span class="pablodraw-gallery-image">\
				<img src="{{attr:~root.baseUrl}}{{attr:previewUrl}}">\
			</span>\
		    <span class="pablodraw-gallery-file-name">{{>fileName}}</span>\
		</a>\
	</li>\
{{/for}}\
</ul>',
		loadingElement: null,
		baseUrl: 'http://gallery.picoe.ca',
		apiUrl: null,
		pack: null,
		params: { page: 0, size: 1000 },
		error: null,
		includeCss: true,
		colorboxSelector: 'a',
		colorboxOptions: {
			rel: 'pack-image',
			fixed: true,
			scalePhotos: false,
			maxHeight: function() { return $(window).height(); },
			maxWidth: function() { return $(window).width(); },
			photo: function() { return $(this).data('type') == 'image'; },
			iframe: function() { return $(this).data('type') != 'image'; },
			onOpen: function() {
				$('html').css({ overflow: 'hidden' });
			},
			onClosed: function() {
				$('html').css({ overflow: 'auto' });
			}
		},
		success: function(data) {
			var template = $.templates(o.template);
			o.result.append(template.render(data));
		},
		loaded: function() {
			o.result.find(o.colorboxSelector).colorbox(o.colorboxOptions);
		}
	};

	var methods = {
		init : function(options) {
			o = $.extend(true, { }, defaults, options);
			o.result = $(this);

			if (o.includeCss)
				$("<link href='" + o.baseUrl + "/external/css' rel='stylesheet'>").appendTo("head");
			
			if (!o.apiUrl)
				o.apiUrl = o.baseUrl + '/api/v0';

			var url = o.apiUrl + '/pack/' + o.pack;
  			url = url + "?" + $.param(o.params);
			if (o.loadingElement) $(o.loadingElement).show();

			$.ajax({
				type: 'get',
				url: url,
				async: true,
				contentType: "application/json",
				crossDomain: true,
				success: function (data) {
					if (data != null) {
						o.params.Page++;
						data = $.extend({ }, data, { baseUrl: o.baseUrl });
						o.success(data);
					}
					else
						o.finished = true;
					if (o.loadingElement) $(o.loadingElement).hide();
					o.inProgress = false;
					if (o.loaded) o.loaded();
				},
				error: o.error
			});
		}
	};

	$.fn.pablogallery = function(methodOrOptions) {
		if ( methods[methodOrOptions] ) {
			return methods[ methodOrOptions ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof methodOrOptions === 'object' || ! methodOrOptions ) {
			// Default to "init"
			return methods.init.apply( this, arguments );
		} else {
			$.error( 'Method ' +  methodOrOptions + ' does not exist on jQuery.pablogallery' );
		}
	};

})( jQuery );
