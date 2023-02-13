/* -----------------------------------------------
-------------------- SHALLOW_JSON ----------------
--------------------------------------------------

This json perser is forked from json2.js

https://gist.github.com/atheken/654510

original notification : 

    http://www.JSON.org/json2.js
    2010-08-25
    Public Domain.
    NO WARRANTY EXPRESSED OR IMPLIED. USE AT YOUR OWN RISK.
    See http://www.JSON.org/js.html
    This code should be minified before deployment.
    See http://javascript.crockford.com/jsmin.html
    USE YOUR OWN COPY. IT IS EXTREMELY UNWISE TO LOAD CODE FROM SERVERS YOU DO
    NOT CONTROL.

 -------------------------------------------------*/

if (typeof SHALLOW_JSON !== "object") {
  SHALLOW_JSON = {};
}

(function () {
  "use strict";

  var rx_one = /^[\],:{}\s]*$/;
  var rx_two = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
  var rx_three = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
  var rx_four = /(?:^|:|,)(?:\s*\[)+/g;
  var rx_escapable = /[\\"\u0000-\u001f\u007f-\u009f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g;
  var rx_dangerous = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g;

  function f(n) {
      return (n < 10)
          ? "0" + n
          : n;
  }

  function this_value() {
      return this.valueOf();
  }

  if (typeof Date.prototype.toJSON !== "function") {
      Date.prototype.toJSON = function () {
          return isFinite(this.valueOf())
              ? (
                  this.getUTCFullYear()
                  + "-"
                  + f(this.getUTCMonth() + 1)
                  + "-"
                  + f(this.getUTCDate())
                  + "T"
                  + f(this.getUTCHours())
                  + ":"
                  + f(this.getUTCMinutes())
                  + ":"
                  + f(this.getUTCSeconds())
                  + "Z"
              )
              : null;
      };

      Boolean.prototype.toJSON = this_value;
      Number.prototype.toJSON = this_value;
      String.prototype.toJSON = this_value;
  }

  var gap;
  var indent;
  var meta;
  var rep;


  function quote(string) {
      rx_escapable.lastIndex = 0;
      return rx_escapable.test(string)
          ? "\"" + string.replace(rx_escapable, function (a) {
              var c = meta[a];
              return typeof c === "string"
                  ? c
                  : "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4);
          }) + "\""
          : "\"" + string + "\"";
  }


  function str(key, holder, value) {
      var i;
      var k;
      var v;
      var length;
      var mind = gap;
      var partial;
      if (
          value
          && typeof value === "object"
          && typeof value.toJSON === "function"
      ) {
          value = value.toJSON(key);
      }

      if (typeof rep === "function") {
          value = rep.call(holder, key, value);
      }

      switch (typeof value) {
      case "string":
          return quote(value);

      case "number":

      return (isFinite(value))
              ? String(value)
              : "null";

      case "boolean":
      case "null":
          return String(value);
      case "object":
          if (!value) {
              return "null";
          }
          gap += indent;
          partial = [];
          if (Object.prototype.toString.apply(value) === "[object Array]") {
              length = value.length;
              for (i = 0; i < length; i += 1) {
                  partial[i] = str(i, value, value[i]) || "null";
              }
              v = partial.length === 0
                  ? "[]"
                  : gap
                      ? (
                          "[\n"
                          + gap
                          + partial.join(",\n" + gap)
                          + "\n"
                          + mind
                          + "]"
                      )
                      : "[" + partial.join(",") + "]";
              gap = mind;
              return v;
          }
      }
  }

  if (typeof SHALLOW_JSON.stringify !== "function") {
      meta = { 
          "\b": "\\b",
          "\t": "\\t",
          "\n": "\\n",
          "\f": "\\f",
          "\r": "\\r",
          "\"": "\\\"",
          "\\": "\\\\"
      };
      SHALLOW_JSON.stringify = function (value, replacer, space) {
          var i;
          gap = "";
          indent = "";
  
          if (typeof space === "number") {
              for (i = 0; i < space; i += 1) {
                  indent += " ";
              }
          } else if (typeof space === "string") {
              indent = space;
          }
  
          rep = replacer;
          if (replacer && typeof replacer !== "function" && (
              typeof replacer !== "object"
              || typeof replacer.length !== "number"
          )) {
              throw new Error("SHALLOW_JSON.stringify");
          }
          return str("", {"": value}, value);
      };
  }
}());

/* -----------------------------------------------
--------------------------------------------------
--------------------------------------------------
*/




/* -----------------------------------------------------
---- convert after effects composition to json data ----
--------------------------------------------------------
*/

var compositions = new Array();
function isExistedComposition (compositionId) {
  for(var i = 0; i < compositions.length; i++) {
    var composition  = compositions[i];
    if (compositionId == composition.id) return true;
  }
  return false;
}

var layers = new Array();
function isExistedLayer (layerId) {
  for(var i = 0; i < layers.length; i++) {
    var layer = layers[i];
    if (layerId == layer.id) return true;
  }
  return false;
}

function parseComposition (composition) {
  var width = composition.width;
  var height = composition.height;
  var compositionId = composition.id;

  var layerIds = [];

  for(var i = 0; i < composition.numLayers; i++) {
    var layer = composition.layer(i + 1);
    if(!layer.enabled
      || layer.guideLayer
      || layer.shy) {
      continue;
    }
    layerIds.push(parseLayer(layer));
  }

  if (!isExistedComposition(compositionId)) {
    compositions.push([
      compositionId,
      width,
      height,
      layerIds,
      composition.comment,
      getCompositionMarkers(composition)
    ]);
  }
  return compositionId;
};

function parseLayer(layer) {
  var name = layer.name;
  var layerId =  "id" + layer.containingComp.id + "-" + layer.index;
  var layerIndex = layer.index;
  var parentIndex = layer.parent != null ? layer.parent.index : -1;
  var footage = null;
  var compositionId = -1;


  if(layer.source instanceof FootageItem) {
    var sourceName = layer.source.name.replace(/^.*[\\\/]/, '');
    var sourceType = "";
    var color = null;

    if(layer.source.mainSource instanceof SolidSource) {
      sourceType = "SolidSource";
      color = layer.source.mainSource.color; 
    }

    if(layer.source.mainSource instanceof FileSource) {
      sourceType = "FileSource";
    }

    footage = [
      sourceName,
      sourceType,
      color
    ];
  } else if (layer.source instanceof CompItem) {
    compositionId = parseComposition(layer.source);
  }


  var properties = null;
  if (layer instanceof AVLayer) {
    properties = [
      getAVLayerTransforms(layer),
      getAVLayerStyles(layer),
      getAVLayerMasks(layer),
    ]
  }


  if (!isExistedLayer(layerId)) {
    layers.push(
      [
        layerId,
        "layer" + layerId, // name
        footage,
        compositionId,
        properties,
        layer.containingComp.id,
        [ // layer options
          layerIndex,
          parentIndex,
          layer.name,
          [ // size
            layer.width,
            layer.height
          ],
          layer.blendingMode, // blend
          layer.comment,
          layer.inPoint,
          layer.outPoint,
          layer.nullLayer
        ]
      ]
    );
  }
  
  return layerId;
}

function getAVLayerTransforms(layer) {
  var transforms = [];
  
  var secondsPerFrame = layer.containingComp.frameDuration;
  var maxFrame = Math.floor(layer.containingComp.duration.toFixed(2) / secondsPerFrame);
  var propertySpecs = [
    layer["Transform"]["Position"],
    layer["Transform"]["Rotation"],
    layer["Transform"]["Scale"],
    layer["Transform"]["Anchor Point"],
    layer["Transform"]["Opacity"],
  ];

  
  {// position
    var propertySpec = propertySpecs[0];

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }

    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      data.push([
        i,// key frame
        [ 
          propertySpec.valueAtTime(time, false)[0],
          propertySpec.valueAtTime(time, false)[1]
        ]
      ]);
    }
    transforms.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }
  
  
  {// Rotation
    var propertySpec = propertySpecs[1];

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }

    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      data.push([
        i,// key frame
        [ 
          propertySpec.valueAtTime(time, false)
        ] 
      ]);
    }
    transforms.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }
  
  {// Scale
    var propertySpec = propertySpecs[2];

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }

    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      data.push([
        i,// key frame
        [ 
          propertySpec.valueAtTime(time, false)[0],
          propertySpec.valueAtTime(time, false)[1]
        ]
      ]);
    }
    transforms.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }
  
  {// anchor
    var propertySpec = propertySpecs[3];

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }

    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      data.push([
        i,// key frame
        [ 
          propertySpec.valueAtTime(time, false)[0],
          propertySpec.valueAtTime(time, false)[1]
        ]
      ]);
    }
    transforms.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }
  
  {// Opacity
    var propertySpec = propertySpecs[4];

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }

    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      data.push([
        i,// key frame
        [ 
          propertySpec.valueAtTime(time, false)
        ]
      ]);
    }
    transforms.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }

  return transforms;
}

function getAVLayerStyles(layer) {
  if (!layer["Layer Styles"]["Color Overlay"]["Color"].isModified
    && !layer["Layer Styles"]["Color Overlay"]["Opacity"].isModified) return [];

  var styles = [];

  var secondsPerFrame = layer.containingComp.frameDuration;
  var maxFrame = Math.floor(layer.containingComp.duration.toFixed(2) / secondsPerFrame);

  {// Color
    var propertySpec = layer["Layer Styles"]["Color Overlay"]["Color"];
    if (propertySpec.isModified) {
      var keys = [];
      for(var i = 1; i <= propertySpec.numKeys; i++) {
        var keyTime = propertySpec.keyTime(i);
        var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
        keys.push(key);
      }
  
      var data = [];
      for(var i = 0; i <= maxFrame; i++) {
        var time = secondsPerFrame * i;
        data.push([
          i,// key frame
          [ 
            propertySpec.valueAtTime(time, false)[0],
            propertySpec.valueAtTime(time, false)[1],
            propertySpec.valueAtTime(time, false)[2]
          ]
        ]);
      }
      styles.push([
        data,
        keys,
        propertySpec.expression
      ]);
    } else {
      styles.push([
        [],
        [],
        ""
      ]);
    }
  }
  
  {// Opacity
    var propertySpec = layer["Layer Styles"]["Color Overlay"]["Opacity"];
    if (propertySpec.isModified) {
      var keys = [];
      for(var i = 1; i <= propertySpec.numKeys; i++) {
        var keyTime = propertySpec.keyTime(i);
        var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
        keys.push(key);
      }
  
      var data = [];
      for(var i = 0; i <= maxFrame; i++) {
        var time = secondsPerFrame * i;
        data.push([
          i,// key frame
          [ 
            propertySpec.valueAtTime(time, false)
          ]
        ]);
      }
      styles.push([
        data,
        keys,
        propertySpec.expression
      ]);
    } else {
      styles.push([
        [],
        [],
        ""
      ]);
    }
  }

  return styles;
}

function getAVLayerMasks(layer) {
  if (layer["Masks"].numProperties == 0) return [];

  var masks = [];

  var secondsPerFrame = layer.containingComp.frameDuration;
  var maxFrame = Math.floor(layer.containingComp.duration.toFixed(2) / secondsPerFrame);

  {// maskOpacity
    var propertySpec = layer["Masks"].property(1).property("maskOpacity");

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }

    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      data.push([
        i,// key frame
        [ 
          propertySpec.valueAtTime(time, false)
        ]
      ]);
    }
    masks.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }
  
  {// maskOffset
    var propertySpec = layer["Masks"].property(1).property("ADBE Mask Offset");

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }

    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      data.push([
        i,// key frame
        [ 
          propertySpec.valueAtTime(time, false)
        ]
      ]);
    }
    masks.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }

  {// maskShape
    var propertySpec = layer["Masks"].property(1).maskShape;

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }


    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      var rawShape = propertySpec.valueAtTime(time, false);

      var vertices = [];
      var inTangents = [];
      var outTangents = [];
      for(var j = 0; j < rawShape.vertices.length; j++) {
        var vector2 = [
          rawShape.vertices[j][0].toFixed(2),
          rawShape.vertices[j][1].toFixed(2)
        ];
        vertices.push(vector2);
      }
      for(var j = 0; j < rawShape.inTangents.length; j++) {
        var vector2 = [
          rawShape.inTangents[j][0].toFixed(2),
          rawShape.inTangents[j][1].toFixed(2)
        ];
        inTangents.push(vector2);
      }
      for(var j = 0; j < rawShape.outTangents.length; j++) {
        var vector2 = [
          rawShape.outTangents[j][0].toFixed(2),
          rawShape.outTangents[j][1].toFixed(2)
        ];
        outTangents.push(vector2);
      }

      data.push([
        i,// key frame
        [ 
          vertices,
          inTangents,
          outTangents
        ]
      ]);
    }

    masks.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }
  
  return [
    masks,
    layer["Masks"].property(1).inverted
  ];
}

function getCompositionMarkers(composition) {
  var marker = composition["markerProperty"];
  if (marker == null) return [];

  var markers = [];

  var secondsPerFrame = composition.frameDuration;
  var maxFrame = Math.floor(composition.duration.toFixed(2) / secondsPerFrame);

  {
    var propertySpec = marker;

    var keys = [];
    for(var i = 1; i <= propertySpec.numKeys; i++) {
      var keyTime = propertySpec.keyTime(i);
      var key = Math.floor(keyTime.toFixed(2) / secondsPerFrame);
      keys.push(key);
    }
    
    var data = [];
    for(var i = 0; i <= maxFrame; i++) {
      var time = secondsPerFrame * i;
      data.push([
        i,// key frame
        [ 
          propertySpec.valueAtTime(time, false)["comment"]
        ]
      ]);
    }
    markers.push([
      data,
      keys,
      propertySpec.expression
    ]);
  }

  return markers;
}

var VersionString = "v1.0.0";
var mainComposition = null;
function exportAll() {
  alert("Please select composition for import");
}

function exportSelected() {
  mainComposition = app.project.activeItem;
  var mainCompositionId = parseComposition(mainComposition);

  var data = [
    mainCompositionId,
    compositions,
    layers,
    (1/mainComposition.frameDuration).toFixed(2),
    VersionString
  ];

  var fileName = mainComposition.name + ".json";
  var file = new File(Folder.desktop.absoluteURI + "/" + fileName);
  file.open("w","TEXT","????");
  file.write(SHALLOW_JSON.stringify(data));
  file.close();
  file.execute();

  alert("Completed. (" + VersionString + ")");
}

(function () {
  if (!(app.project.activeItem instanceof CompItem)) {
    exportAll();
  } else {
    exportSelected();
  }
})();
