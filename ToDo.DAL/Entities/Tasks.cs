using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ToDo.DAL.Entities;

public partial class Tasks
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [StringLength(20)]
    public string Priority { get; set; } = null!;

    [StringLength(30)]
    public string Status { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Tasks")]
    public virtual User User { get; set; } = null!;
}
