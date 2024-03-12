using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSqlKata
{
	internal class AlarmItem
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>        
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the name of the item.
		/// </summary>
		/// <value>
		/// The name of the item.
		/// </value>
		public string ItemName { get; set; }

		/// <summary>
		/// Gets or sets the ack status.
		/// </summary>
		/// <value>
		/// The ack status.
		/// </value>
		public int? AckStatus { get; set; }

		/// <summary>
		/// Gets or sets the ack required.
		/// </summary>
		/// <value>
		/// The ack required.
		/// </value>
		public int? AckRequired { get; set; }

		/// <summary>
		/// Gets or sets the name of the tag.
		/// </summary>
		/// <value>
		/// The name of the tag.
		/// </value>
		public string TagName { get; set; }

		/// <summary>
		/// Gets or sets the condition.
		/// </summary>
		/// <value>
		/// The condition.
		/// </value>
		public string Condition { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the value as string.
		/// </summary>
		/// <value>
		/// The value as string.
		/// </value>
		[NotMapped]
		public string ValueAsString { get; set; }

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the name of the group.
		/// </summary>
		/// <value>
		/// The name of the group.
		/// </value>
		public string GroupName { get; set; }

		/// <summary>
		/// Gets or sets the area.
		/// </summary>
		/// <value>
		/// The area.
		/// </value>
		public string Area { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		public int? State { get; set; }

		/// <summary>
		/// Gets or sets the priority.
		/// </summary>
		/// <value>
		/// The priority.
		/// </value>
		public int? Priority { get; set; }

		/// <summary>
		/// Gets or sets the active time_ ticks.
		/// </summary>
		/// <value>
		/// The active time_ ticks.
		/// </value>
		public long ActiveTime_Ticks { get; set; }

		/// <summary>
		/// Gets or sets the norm time_ ticks.
		/// </summary>
		/// <value>
		/// The norm time_ ticks.
		/// </value>
		public long? NormTime_Ticks { get; set; }

		/// <summary>
		/// Gets or sets the ack time_ ticks.
		/// </summary>
		/// <value>
		/// The ack time_ ticks.
		/// </value>
		public long? AckTime_Ticks { get; set; }

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>
		/// The name of the user.
		/// </value>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the comments.
		/// </summary>
		/// <value>
		/// The comments.
		/// </value>
		public string Comments { get; set; }

		/// <summary>
		/// Gets or sets the color fg.
		/// </summary>
		/// <value>
		/// The color fg.
		/// </value>
		public string ColorFG { get; set; }

		/// <summary>
		/// Gets or sets the color bg.
		/// </summary>
		/// <value>
		/// The color bg.
		/// </value>
		public string ColorBG { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [not synchronize].
		/// </summary>
		/// <value>
		///   <c>true</c> if [not synchronize]; otherwise, <c>false</c>.
		/// </value>
		public bool NotSync { get; set; }

		/// <summary>
		/// Gets or sets the tag level.
		/// </summary>
		/// <value>
		/// The tag level.
		/// </value>
		public string Level { get; set; }

		/// <summary>
		/// Gets or sets the active local time.
		/// </summary>
		/// <value>
		/// The active local time.
		/// </value>
		public DateTime ActiveLocalTime { get; set; }

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>
		/// The duration.
		/// </value>
		public long? Duration { get; set; }

		/// <summary>
		/// Gets or sets the category.
		/// </summary>
		/// <value>
		/// The category.
		/// </value>
		public long? Category { get; set; }

		/// <summary>
		/// Gets or sets the date created_ ticks.
		/// </summary>
		/// <value>
		/// The date created_ ticks.
		/// </value>
		public long DateCreated_Ticks { get; set; }

		///// <summary>
		///// Gets the date created date time.
		///// </summary>
		///// <value>
		///// The date created date time.
		///// </value>
		//[NotMapped]
		//public DateTime DateCreated_DateTime => new DateTime(DateCreated_Ticks);

		/// <summary>
		/// Gets or sets the aux value.
		/// </summary>
		/// <value>
		/// The aux value.
		/// </value>
		public string AuxValue { get; set; }

		/// <summary>
		/// Gets or sets the alarm limit.
		/// </summary>
		/// <value>
		/// The alarm limit.
		/// </value>
		public double AlarmLimit { get; set; }

		/// <summary>
		/// Gets or sets the previous value.
		/// </summary>
		/// <value>
		/// The previous value.
		/// </value>
		public string PreviousValue { get; set; }
	}
}
