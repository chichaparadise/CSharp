Лабораторная работа №4

Извините что без видео, но без вай фая бандикам особо не скачаешь а запись экрана windows - проще повеситься, чем захватить нужное окно
Лаба разбита на 6 проектов:
* Models - модельки используемые в лабе(Person - человек, Employee - сотрудник, Error - ошибка, наследуемая от Exception)
* DataAccessLayer предназначен для работы с бд, класс DBReader считывает данные из бд(сотрудники и их личные данные), класс DBWriter записывает данные в бд(адаптирован под запись ошибок,
 можно было сделать его абстрактным или интрефейсом для более общей реализации, но я и так опоздал со сроком)
* ServiceLayer - связующее звено между DAL и самой службой, class FileTransefer реализует передачу потока stream по пути string destination, XMLGenerator - его функционал
 выпяд ли вас удивит
* ConfigurationManager - доработан из 3 лабы для получения любых сеттингов для любого проекта
* DataManagerService - основная служба, берёт всех сотрудников из бд, запихивает в xml файл и xsd схему и отправляет по пути FTP.Source указанному в Settings.FileWatcherSourceFolder
* FileManagerService - 3 лаба
Отедльно кинул файлы конфигураций, хотя они должны находиться рядом с exe-шкой