Джун написал код. И этот код даже компилируется. Но скажем честно, это единственное его достоинство.

Попробуйте переписать код таким образом, чтобы исправить ошибки, допущенные программистом, но при этом сохранив основную бизнес логику.

Для упрощения будем считать, что ошибки допущены только в классе **BrokenService**, остальной код менять не надо.

# Процесс выполнения
1. Проект все таки не комплируется. Создал шаблонный Asp.Net.Core проект и по принципу "Найди 10 отличий" добавил все недостающее в этот проект. Он заработал.
2. Почему Asp.Net.Core, потому файл stylecop.json навел на мысли
3. Добавил контроллеры и страницу
4. Добавил интерфейс для BrokenService и реализовал интерфейс сервиса лицензий и провайдера лицензий
5. Добавил ReportController для инкапсуялции логики работы с отчетами
6. Добавил заполнение GetReportRequest из страницы для точечной проверки
7. Смерджил ветку origin/fast-refactor с main веткой

Другие замечания, которые либо не знал как исправлять, либо нужно доп. время:
1. GetReportRequest возможно лучше сделать структурой, хотя ни одного примера такого не нашел
2. Возможно можно как-то красиво разделить запросы на получение репортов по страницам, чтобы использовать PageModel и привязку данных, но не знаю как
3. Для таблицы с пагинацией возможно лучше использовать js datatable, он обладает возможностью асинхронной загрузки данных с сервера (под капотом предоставляет пагинацию)
4. Ссылка на js datatable https://datatables.net/. Использовал в проекте, интересная вещь, но Blazor немного удобнее, хоть мы от него и отказались, по причине что Blazor слишком уж шаманит жизенные циклы сервисов в Asp Net Core, но это отдельная история
5. Запрос на пользователей и их лицензии можно как-то объеденить в один и доставать из БД сразу через аггрегацию, но как быстро сделать - не знаю
6. 

# Ветка origin/fast-refactor
1. Быстрый рефакторинг только BrokenService буз запуска и попытки убедиться, что бизнес логика цела.