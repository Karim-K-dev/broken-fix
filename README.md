Джун написал код. И этот код даже компилируется. Но скажем честно, это единственное его достоинство.

Попробуйте переписать код таким образом, чтобы исправить ошибки, допущенные программистом, но при этом сохранив основную бизнес логику.

Для упрощения будем считать, что ошибки допущены только в классе **BrokenService**, остальной код менять не надо.

# Описание
ReportService это бывший BrokenService
SQL Lite создается автоматически, можно добавлять и удалять пользователей
Связи между таблицами нету


### Добавить пользователя
Users -> Add User
### Посмотреть отчет
Show Report или User -> Show Report
Можно не вводить поля, вроде Domain Id или Page Size, но они тоже работают

# Процесс выполнения
1. Проект все таки не комплируется. Создал шаблонный Asp.Net.Core проект и по принципу "Найди 10 отличий" добавил все недостающее в этот проект. Он заработал.
2. Почему Asp.Net.Core, потому файл stylecop.json навел на мысли
3. Добавил контроллеры и страницу
4. Добавил интерфейс для BrokenService и реализовал интерфейс сервиса лицензий и провайдера лицензий
5. Добавил ReportController для инкапсуялции логики работы с отчетами
6. Добавил заполнение GetReportRequest из страницы для точечной проверки
7. Смерджил ветку origin/fast-refactor с main веткой
8. Вынес настройку таймаута в appsettings.json и сразу заовверайдил значение на 5000 (как в коде, хардкод удалил)
9. Конфигурацию сервиса лицензий инкапсулировал в сервис лицензий в конструкторе, чтобы поддержать инкапсуляцию, и как следствие...
10. Удалил LicenseServiceProvider так он теперь полностью не нужен
11. Удалил трай кетч для сервиса лицензий, т.к. содержит избыточную информацию и никак не обрабатывает исключение. Чтобы понять, что дело в сервисе лицензий, достаточно будет посмотреть стэк трейс
12. Блок обработки лицензий упростил и инкапуслировал в сервис лицензий. Он по факту берет лицензии пользователей, убрал кучу алокаций на LINQ
13. А потом и вовсе убрал словарь userLicense, так как коллекция LicenseInfo уже содержит необходимые UserId (Guid пользователя)
14. А потом и инкапсулировал всю логику получения типа лицензий внутри сервиса лицензий (инкапсуляция + упрощение)
15. Вынес настройку пользовательской БД в appsettings.json для удобной настройки
16. Унес в расширения работу с БД, чтобы она не зависила от брокен сервиса и была доступна везде, где доступна коллекция Users из БД
17. Добавил подсчет лицензированных пользователей в сервисе лицензий
18. Добавил использование CancellationToken при запросе отчета
19. Добавил отдельную страницу для работы с БД пользователей
20. Добавил отдельную страницу для показа отчетов


Другие замечания, которые либо не знал как исправлять, либо нужно доп. время (в процессе работы я бы создал задачи в бэклог на эти темы и отдавал бы стажерам):
1. GetReportRequest возможно лучше сделать структурой, хотя ни одного примера такого не нашел
2. Возможно можно как-то красиво разделить запросы на получение репортов по страницам, чтобы использовать PageModel и привязку данных, но не знаю как
3. Для таблицы с пагинацией возможно лучше использовать js datatable, он обладает возможностью асинхронной загрузки данных с сервера (под капотом предоставляет пагинацию)
4. Ссылка на js datatable https://datatables.net/. Использовал в проекте, интересная вещь, но Blazor немного удобнее, хоть мы от него и отказались, по причине что Blazor слишком уж шаманит жизенные циклы сервисов в Asp Net Core, но это отдельная история
5. Запрос на пользователей и их лицензии можно как-то объединить в один и доставать из БД сразу через аггрегацию, но как быстро сделать - не знаю
6. В идеале для ID везде использовать не Guid, а long, т.к. ID long позволяет скипать ползователей по SkipWhile, где он в условие Id <= SkipUsers. Это значительно быстрее
7. Переписал запросы пользователей без использования функций, чтобы обработка выполнялась со стороны БД и не нагружала клиент
8. Не понятно как сделать, чтобы токен стрелял ошибкой по среди выполнения потока как в офф доке https://docs.microsoft.com/ru-ru/dotnet/csharp/programming-guide/concepts/async/cancel-async-tasks-after-a-period-of-time (тут бы я наверное спросил знающих)

# Ветка origin/fast-refactor
1. Быстрый рефакторинг только BrokenService без запуска и попытки убедиться, что бизнес логика цела.