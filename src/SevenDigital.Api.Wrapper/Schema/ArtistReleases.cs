﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SevenDigital.Api.Wrapper.Schema.Attributes;

namespace SevenDigital.Api.Wrapper.Schema
{
	[Serializable]
	[ApiEndpoint("artist/releases")]
	[XmlRoot("releases")]
	public class ArtistReleases
	{
		[XmlElement("page")]
		public int Page { get; set; }

		[XmlElement("pageSize")]
		public int PageSize { get; set; }

		[XmlElement("totalItems")]
		public int TotalItems { get; set; }

		[XmlElement("release")]
		public List<Release> Releases { get; set; }
	}
}