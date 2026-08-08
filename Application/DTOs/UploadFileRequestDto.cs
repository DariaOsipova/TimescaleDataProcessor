namespace Application.DTOs
{
    public class UploadResultDto
    { // сообщает клиенту, что произошло после загрузки файла
        public string FileName { get; set; } = string.Empty;
        public int RecordsCount { get; set; } // количество записей, которое было обработано или найдено
        public bool IsNewFile { get; set; }
    }
}