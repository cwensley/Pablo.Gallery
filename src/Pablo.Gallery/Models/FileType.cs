using System;
using System.Collections.Generic;

namespace Pablo.Gallery.Models
{
	public class FileType
	{
		public static readonly FileType Image = new FileType("image",
			                                        new FileFormat("icon", "ico"),
			                                        new FileFormat("pcx", "pcx"),
			                                        new FileFormat("gif", "gif"),
			                                        new FileFormat("jpeg", "jpg", "jpeg"),
			                                        new FileFormat("png", "png"),
			                                        new FileFormat("tga", "tga"),
			                                        new FileFormat("lbm", "lbm"),
			                                        new FileFormat("iff", "iff"),
			                                        new FileFormat("tiff", "tiff", "tif"),
			                                        new FileFormat("bmp", "bmp")
		                                        );
		public static readonly FileType Character = new FileType("character", 
			                                            new FileFormat("ansi", "ans", "ansi", "diz", "mem", "cia", "drk", "ice", "tri", "tas",
				                                            "tag", "lit", "vor", "uni", "vnt", "bad", "crp", "cma", "rel", "sca", "sui", "wbl", 
				                                            "srg", "lgo", "vib", "sap", "pur", "nat", "jus", "itr", "imp", "ali", "grp", "fwk",
				                                            "mft", "min", "nwa", "nit", "axe", "ace", "ete", "evl", "sik", "tly", "tsd", "wkd",
				                                            "air", "hyp", "hpe", "rpm", "mir", "sda", "ech", "ltd", "pop", "ioa", "lgc", "chs",
				                                            "atm", "tdd", "art", "ovt", "dcp", "fus", "rca", "log", "viv", "oil", "got", "mad",
				                                            "org", "die", "rib", "rev", "bdp", "ind", "xpo", "ill", "---", "___", "···", "¨¨¨"
			                                            ),
			                                            new FileFormat("ascii", "asc", "txt", "nfo"),
			                                            new FileFormat("bin", "bin"),
			                                            new FileFormat("xbin", "xb", "xbin"),
			                                            new FileFormat("adf", "adf"),
			                                            new FileFormat("idf", "idf"),
			                                            new FileFormat("avt", "avt"),
			                                            new FileFormat("seq", "seq", "cg"),
			                                            new FileFormat("ctrla", "msg"),
			                                            new FileFormat("tundra", "tnd")
		                                            );
		public static readonly FileType Text = new FileType("text",
			                                       new FileFormat("html", "html"),
			                                       new FileFormat("css", "css"),
			                                       new FileFormat("csv", "csv"),
			                                       new FileFormat("javascript", "js"),
			                                       new FileFormat("plain", "cs", "vb", "c"),
			                                       new FileFormat("xml", "xml")
		                                       );
		public static readonly FileType Rip = new FileType("rip", 
			                                      new FileFormat("rip", "rip")
		                                      );
		public static readonly FileType Binary = new FileType("binary",
			                                         new FileFormat("exe", "exe", "com"),
			                                         new FileFormat("raw", "raw"),
			                                         new FileFormat("library", "dll", "lib", "obj"),
			                                         new FileFormat("pif", "pif"),
			                                         new FileFormat("bgi", "bgi"),
			                                         new FileFormat("db", "db"),
			                                         new FileFormat("java", "class"),
			                                         new FileFormat("font", "fnt", "fon", "ttf", "chr"),
			                                         new FileFormat("wad", "wad")
		                                         );
		public static readonly FileType Application = new FileType("application",
			                                              new FileFormat("illustrator", "ai"),
			                                              new FileFormat("json", "json"),
			                                              new FileFormat("x-shockwave-flash", "swf"),
			                                              new FileFormat("octet-stream", "dat", "000", "001", "002", "003")
		                                              );
		public static readonly FileType Audio = new FileType("audio",
			                                        new FileFormat("st", "s3m", "stm"),
			                                        new FileFormat("it", "it"),
			                                        new FileFormat("xm", "xm"),
			                                        new FileFormat("mod", "mod"),
			                                        new FileFormat("mid", "mid"),
			                                        new FileFormat("mp3", "mp3"),
			                                        new FileFormat("669", "669"),
			                                        new FileFormat("voc", "voc"),
			                                        new FileFormat("mtm", "mtm"),
			                                        new FileFormat("dmf", "dmf"),
			                                        new FileFormat("ult", "ult"),
			                                        new FileFormat("far", "far"),
			                                        new FileFormat("wav", "wav")
		                                        );
		public static readonly FileType Video = new FileType("video",
			                                        new FileFormat("flic", "fli", "flc"),
			                                        new FileFormat("flash", "flv", "f4v"),
			                                        new FileFormat("avi", "avi"),
			                                        new FileFormat("smk", "smk"),
			                                        new FileFormat("wmv", "wmv"),
			                                        new FileFormat("mov", "mov"),
			                                        new FileFormat("mpeg", "mp4", "mpg", "mpeg"),
			                                        new FileFormat("mkv", "mkv")
		                                        );
		public static readonly FileType Archive = new FileType("archive",
			                                          new FileFormat("zip", "zip"),
			                                          new FileFormat("arj", "arj"),
			                                          new FileFormat("rar", "rar"),
			                                          new FileFormat("sit", "sit"),
			                                          new FileFormat("apk", "apk")
		                                          );

		public string Name { get; private set; }

		public IEnumerable<FileFormat> Formats { get; private set; }

		public FileType(string name, params FileFormat[] formats)
		{
			Name = name;
			Formats = formats;
			foreach (var format in formats)
			{
				format.Type = this;
			}
		}

		static readonly Dictionary<string, FileType> types = new Dictionary<string, FileType>
		{
			{ Image.Name, Image },
			{ Character.Name, Character },
			{ Text.Name, Text },
			{ Rip.Name, Rip },
			{ Binary.Name, Binary },
			{ Audio.Name, Audio },
			{ Video.Name, Video },
			{ Application.Name, Application },
			{ Archive.Name, Archive }
		};

		public static IEnumerable<FileType> Types
		{
			get { return types.Values; }
		}

		public static FileType Find(string typeName)
		{
			FileType type;
			return types.TryGetValue(typeName, out type) ? type : null;
		}
	}
}

