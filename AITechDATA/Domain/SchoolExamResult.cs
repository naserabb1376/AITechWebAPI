namespace AITechDATA.Domain
{
    public class SchoolExamResult
    {
        public long ID { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; } = true;

        public long SchoolRegistrationId { get; set; }
        public SchoolRegistration SchoolRegistration { get; set; }

        public string TenantKey { get; set; } = "takinschool";
        public string ExamKey { get; set; } = "takinschool-grade4-entrance-2026";
        public string Grade { get; set; } = "چهارم";
        public int TotalQuestions { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }
        public int BlankCount { get; set; }
        public decimal ScorePercent { get; set; }
        public bool? IsAccepted { get; set; }
        public string AcceptanceMessage { get; set; } = "";
        public bool? IsFinalAccepted { get; set; }
        public string FinalAcceptanceStatus { get; set; } = "";
        public string AnswersJson { get; set; } = "{}";
        public string SectionScoresJson { get; set; } = "[]";
        public string EntrySource { get; set; } = "Manual";
        public decimal? OmrConfidence { get; set; }
        public bool OmrReviewRequired { get; set; }
        public string? RawOmrJson { get; set; }
        public bool Published { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
