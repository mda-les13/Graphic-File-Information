# Computer-graphics-programming-Lab-2

## Описание
Проект PKG_Lab2 представляет собой приложение на WPF, предназначенное для извлечения и отображения метаданных изображений из выбранной папки или отдельного файла. Приложение поддерживает различные форматы изображений, такие как JPEG, PNG, BMP, GIF и TIFF.

## Классы и методы
- Метод SelectFolder_Click
Открывает диалог выбора папки, извлекает метаданные для всех изображений в выбранной папке и заполняет таблицу MetadataGrid.
- Метод SelectFolder_Click1
Открывает диалог выбора файла, извлекает метаданные для выбранного изображения и заполняет таблицу MetadataGrid.
### Методы для извлечения метаданных
- GetImageResolution(string imagePath): Возвращает разрешение изображения в формате "ширина x высота".
- GetImageDpi(string imagePath): Возвращает значение DPI изображения.
- GetImageColorDepth(string imagePath): Возвращает глубину цвета изображения в битах на пиксель.
- GetFileSize(string imagePath): Возвращает размер файла в мегабайтах.
- GetFileCreationDate(string filePath): Возвращает дату создания файла.
- GetCompressionRatio(string imagePath): Возвращает коэффициент сжатия изображения.
- GetUniqueColorCount(string imagePath): Возвращает количество уникальных цветов в изображении.
### Методы для работы с изображениями
- GetEncoder(ImageFormat format): Возвращает кодек для заданного формата изображения.
- BitmapFromSource(BitmapSource bitmapsource): Преобразует объект BitmapSource в Bitmap.
### Класс ImageMetadata
##### Свойства
- FileName: Имя файла изображения.
- Dimensions: Размеры изображения.
- Resolution: Разрешение изображения.
- ColorDepth: Глубина цвета изображения.
- Compression: Коэффициент сжатия изображения.
- ColorCount: Количество уникальных цветов в изображении.
- Size: Размер файла изображения.
- Date: Дата создания файла.
## Использование
Запустите приложение.
Нажмите кнопку для выбора папки или файла.
Приложение извлечет метаданные и отобразит их в таблице.
## Зависимости
MetadataExtractor: Библиотека для извлечения метаданных из изображений.
## Примечания
Приложение поддерживает только определенные форматы изображений. Убедитесь, что выбранные файлы соответствуют поддерживаемым форматам.
Если необходимо изменить или добавить функциональность, обратите внимание на структуру методов и классов.
## Лицензия
Этот проект доступен под лицензией MIT.
