using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataEntity
{
	public class Review
	{
		int mid;
		string username;
		string starLevel;
		string content;
		string postTime;
		public int MerchantId
		{
			get { return mid; }
			set { mid = value; }
		}
		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		public string StarLevel
		{
			get { return starLevel; }
			set { starLevel = value; }
		}

		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		public string PostTime
		{
			get { return postTime; }
			set { postTime = value; }
		}
	}
}
