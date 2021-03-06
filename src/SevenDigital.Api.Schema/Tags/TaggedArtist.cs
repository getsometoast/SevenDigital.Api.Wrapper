﻿using System.Xml.Serialization;
using SevenDigital.Api.Schema.ArtistEndpoint;

namespace SevenDigital.Api.Schema.Tags
{
	[XmlRoot("taggedItem")]
	public class TaggedArtist
	{
		[XmlElement("artist")]
		public Artist Artist { get; set; }
	}
}