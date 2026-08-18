using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ToDo.DAL.Entities;

namespace ToDo.BLL.Dto.Project
{
    public class UpdateprojectDto
    {
        public int ID { get; set; }
        [StringLength(150)]
        public string Title { get; set; } = null!;

        [StringLength(1000)]
        public string? Description { get; set; }

        

        [StringLength(20)]
        public string Priority { get; set; } = null!;

        [StringLength(30)]
        public string Status { get; set; } = null!;

        public int UserId { get; set; }

       

        
    }
}
