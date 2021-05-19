using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class ReviewModel
    {
        public int ReviewId { get; set; }
        public int? ReviewCategoryId { get; set; }
        public string Category { get; set; }
        public decimal Score { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string Message { get; set; }
    }
}
