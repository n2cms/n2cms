@echo off

echo Deleting deploy files
rmdir /S/Q .\Deploy
rmdir /S/Q ..\Templates\Tasks\Deploy
echo Deleting more files
rmdir /S/Q ..\Core\N2.Tests\bin
rmdir /S/Q ..\Core\N2.Tests\obj
rmdir /S/Q ..\Core\N2.MediumTrust\bin
rmdir /S/Q ..\Core\N2.MediumTrust\obj
rmdir /S/Q ..\Core\Edit\N2.Edit.Tests\bin
rmdir /S/Q ..\Core\Edit\N2.Edit.Tests\obj
rmdir /S/Q ..\Core\N2\bin
rmdir /S/Q ..\Core\N2\obj
rmdir /S/Q ..\Core\N2.Parts\bin
rmdir /S/Q ..\Core\N2.Parts\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\obj
echo Deleting templates
rmdir /S/Q ..\Templates\N2.Templates\bin
rmdir /S/Q ..\Templates\N2.Templates\obj
rmdir /S/Q ..\Templates\Security\bin
rmdir /S/Q ..\Templates\Security\obj
rmdir /S/Q ..\Templates\SEO\bin
rmdir /S/Q ..\Templates\SEO\obj
rmdir /S/Q ..\Templates\Survey\bin
rmdir /S/Q ..\Templates\Survey\obj
rmdir /S/Q ..\Templates\Syndication\bin
rmdir /S/Q ..\Templates\Syndication\obj
rmdir /S/Q ..\Templates\UI\bin
rmdir /S/Q ..\Templates\UI\obj
rmdir /S/Q ..\Templates\UI\Advertisement\bin
rmdir /S/Q ..\Templates\UI\Advertisement\obj
rmdir /S/Q ..\Templates\UI\Calendar\bin
rmdir /S/Q ..\Templates\UI\Calendar\obj
rmdir /S/Q ..\Templates\UI\Faq\bin
rmdir /S/Q ..\Templates\UI\Faq\obj
rmdir /S/Q ..\Templates\UI\Form\bin
rmdir /S/Q ..\Templates\UI\Form\obj
rmdir /S/Q ..\Templates\UI\ImageGallery\bin
rmdir /S/Q ..\Templates\UI\ImageGallery\obj
rmdir /S/Q ..\Templates\UI\News\bin
rmdir /S/Q ..\Templates\UI\News\obj
rmdir /S/Q ..\Templates\UI\Poll\bin
rmdir /S/Q ..\Templates\UI\Poll\obj
rmdir /S/Q ..\Templates\UI\Scrum\bin
rmdir /S/Q ..\Templates\UI\Scrum\obj
rmdir /S/Q ..\Templates\UI\RSS\bin
rmdir /S/Q ..\Templates\UI\RSS\obj
rmdir /S/Q ..\Templates\UI\Search\bin
rmdir /S/Q ..\Templates\UI\Search\obj
echo Deleting edit
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Install\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Install\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\LinkTracker\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\LinkTracker\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Membership\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Membership\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Security\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Security\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Trash\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Trash\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Wizard\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Wizard\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Versions\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Versions\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Export\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Export\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\obj

echo Done

pause