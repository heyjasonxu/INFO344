<?php

class Player {
	private $name;
    private $gp;
    private $team;
    private $ppg;
    private $reb;
    private $ast;

	function __construct($player) {
        $this->name = $player['Name'];
        $this->gp = $player['GP'];
        $this->team = $player['Team'];
        $this->ppg = $player['PPG'];
        $this->reb = $player['Tot(Reb)'];
        $this->ast = $player['Ast'];
	}

	function getName() {
    return $this->name;
  }

  function getGp() {
    return $this->gp;
  }

  function getTeam() {
    return $this->team;
  }

  function getPpg() {
    return $this->ppg;
  }

  function getReb() {
   return $this->reb;
  }

  function getAst() {
    return $this->ast;
  }
  
  function playerJson() {
    return array("name" => $this->getName(),
    "gp" => $this->getGp(),
    "team" => $this->getTeam(),
    "ppg" => $this->getPpg(),
    "reb" => $this->getReb(),
    "ast" => $this->getAst());
  }
}

?>