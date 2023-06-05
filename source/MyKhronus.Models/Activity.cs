namespace MyKhronus.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Activity
{
    public Activity(string name)
    {
        ActivityId = Guid.NewGuid();
        Name = name;
        Records = new List<ActivityRecord>();
    }

    public Guid ActivityId { get; private set; }

    [Required]
    public string Name { get; set; }

    public IEnumerable<ActivityRecord> Records { get; private set; }

    public ActivityRecord AddRecord(DateTime recordDate)
    {
        var newActivityRecord = new ActivityRecord(this, recordDate);

        var currentRecords = Records.ToList();

        currentRecords.Add(newActivityRecord);

        Records = currentRecords;

        return newActivityRecord;
    }

}
