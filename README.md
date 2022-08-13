# AnimaShoesApp
VKBot&amp;EmailBot

Xamarin Android application which contains two bots - VKBot and EmailBot. 

Application works in the background as android service. 

You need to fill required fields for authorization and then activate it on the main page by switching toggle button.

## VKBot

First one is working on longpoll Api. It reads id of every person who's messaging a vkgroup and responds with greet.

Client needs to write "чатбот" and bot will respond with a few number of buttons which client can interact with. 

All answers are rewritable and can be changed for your needs.

## EmailBot

Second works on MailKit Api. It parses order information and responds.

All answers are rewritable too. The thing is, they are hardcodded into app for customer purpose only.
