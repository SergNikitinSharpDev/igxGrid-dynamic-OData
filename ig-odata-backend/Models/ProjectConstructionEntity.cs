using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class ProjectConstructionEntity
    {
        public virtual Guid id { get; set; }
        [Display(Name = @"Наименование")]
        public virtual string name { get; set; }
        [Display(Name = @"Номер договора")]
        public virtual string business_id { get; set; }
        [Display(Name = @"Владелец")]
        public virtual Guid? owner_id { get; set; }
        [Display(Name = @"Создание")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        [Column(TypeName = "timestamptz")]
        public virtual DateTime? create_date { get; set; }

        public string address { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}