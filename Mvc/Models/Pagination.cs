﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SitefinityWebApp.Mvc.Models
{
	public class Pagination
	{

		public int CurrentPage { get; set; }

		public long TotalItems { get; set; }

		[Display(Name = "Results Per Page")]
		public int ItemsPerPage { get; set; }

		public int NextPage
		{
			get { return Math.Min(TotalPages, CurrentPage + 1); }
		}

		public bool ShowNextPage
		{
			get { return CurrentPage < TotalPages; }
		}

		public int PreviousPage
		{
			get { return Math.Max(1, CurrentPage - 1); }
		}

		public bool ShowPreviousPage
		{
			get { return CurrentPage > 1; }
		}

		public int TotalPages
		{
			get
			{
				if ((decimal)ItemsPerPage <= 0)
				{
					ItemsPerPage = 1;
				}
				return Convert.ToInt32(Math.Ceiling(TotalItems / (decimal)ItemsPerPage));
			}
		}

		public IEnumerable<int> LinkedPages
		{
			get
			{
				var first = Math.Max(1, CurrentPage - 8);
				var last = Math.Min(TotalPages, CurrentPage + 8);

				return Enumerable.Range(first, Math.Max(last - first + 1, 1));
			}
		}

		public SelectList ItemsPerPageList { get; set; }

		public object RouteValues { get; set; }
	}
}