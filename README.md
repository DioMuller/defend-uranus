defend-uranus
=============

Star Control 2 like game.

Objetivo
=============
Construir um jogo vetorial similar ao modo melee do Ur-Quan Masters (Star Control 2)
* Jogo para dois jogadores;
* Pode ser feito em equipes de 1 até 3 pessoas

Descrição do jogo
=============
*Consiste numa arena, onde 2 naves diferentes devem lutar até a morte
*Cada nave tem:
	*Um marcador de energia (power);
	*Um marcador de vida (life);
	*Um canhão;
	*Um golpe especial.
*Cada nave possui dois tipos de movimentação:
	*Propulsão: Empurra a nave para frente;
	*Rotação: Muda a direção para onde a nave aponta;
*A movimentação das naves seguem as leis de newton no vácuo.
	*Naves de massas diferentes devem ser mais fáceis/difíceis de se colocar em movimento
	*Uma vez em movimento, só outra força irá freá-lo	
*O golpe especial de uma das naves deve, necessariamente, ser um míssil teleguiado (através do steering behavior pursuit);
*O míssil teleguiado fica um tempo pré-definido na tela (calibre esse tempo para não ser muito curto ou longo, e manter o jogo divertido);
*A explosão do míssil deve atingir também a nave que o atirou, caso seja muito próxima.
*O poder da outra deve ser defensivo, opções:
	*Ficar invisível ao míssil por um período de tempo. Durante esse período, o míssil fica em wander;
	*Jogar flares, que repelem o míssil com flee;
	*Jogar um anti-míssil: Um míssil que tenta interceptar o míssil original
	*Qualquer outra opção que também use um steering behavior;
*Asteróides podem aparecer no cenário a qualquer momento, se deslocando de um canto até outro;
*Tiros são capazes de destruí-los;
*Naves podem colidir com eles, causando a destruição do asteróide e perda de vida;
*Não há colisão entre as naves;
	*A colisão da nave com o próprio tiro ou poder especial é opcional;
*Cada equipe deve implementar 1 nave + 1 para cada membro;
*As naves devem ser diferentes em seus poderes, massa e habilidades;
*Procure balancear o game para torná-lo divertido;
*A câmera não precisa se ter zoom, como no jogo original.

Avaliação
=============
*A avaliação do jogo se dará por:
	*Uso correto de vetores para modelar o problema;
	*Uso correto dos steering behaviors;
	*Uso de efeitos com partículas;
*Presença de gráficos ou som que colaborem com a sensação física do jogo;
*Implementação de todos os requisitos do trabalho;
*Deve ser anexado um arquivo descrevendo as classes do jogo e como cada técnica foi utilizada.
*Consulte o prazo de entrega no Eureka.
*O jogo pode ser implementado em C++ ou Xna 4.0.
*Não é permitido usar uma engine de física para fazer a movimentação, colisão ou behaviors.
*O tema não precisa ser espacial, você pode adapta-lo para ser no “fundo do mar”. Desde que lá também não haja resistência ao movimento por parte da água.

Referências
=============

*Jogo Ur-Quan Masters (Remake do Star Control II)
*Entrar em modo Melee
	http://sc2.sourceforge.net/downloads.php

*Procure jogar o jogo com seu colega de equipe até entender bem sua mecânica.