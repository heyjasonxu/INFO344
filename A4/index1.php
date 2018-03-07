
<div style="text-align: center;"><img src="nba-stats-logo.svg" alt="NBA 2015-16 Stats" width="250"></div> <br>
<form action="index.php" method="get" align="center">
Name: <input type="text" name="name">
<input type="submit">

<?php
require('player.php');

if(isset($_GET['name'])) {
    $term =$_GET["name"];

    try {
        $conn = new PDO('mysql:host=info344db.cchp6b1kx7uy.us-west-2.rds.amazonaws.com;dbname=info344db', 'heyjasonxu', 'Xuweiming34');
        $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);    
        
        $stmt = $conn->prepare("SELECT * FROM `15-16nba_stats` WHERE Name LIKE '%$term%'");
        
        $stmt->execute();
        // if($stmt->rowCount() == 1) {
            while($row = $stmt->fetch()) {
            $player = new Player($row);
            // $data = ["name" => $player->getName(),
            //         "team" => $player->getTeam(),
            //         "gp" => $player->getGp(),
            //         "ppg" => $player->getPpg(),
            //         "reb" => $player->getReb(),
            //         "Ast" => $player->getAst()];
            $data = ["name" => "test"];
                    header('Content-Type: application/json');
                    echo json_encode( $data );
        // }
            }
                
        ?>

        <!-- <table style="width:50%" border="1" align="center">
        <tr>
              <th>Name</th>
              <th>Team</th>
              <th>GP</th> 
              <th>PPG</th>
              <th>REB</th>
              <th>AST</th>
            </tr> -->
        <?php

        while($row = $stmt->fetch()) {
            $player1 = new Player($row);
            $player_name = $player1->getName();
            ?>
            <!-- <tr>
              <td><?php echo $player1->getName();?></td>
              <td><?php echo $player1->getTeam();?></td>
              <td><?php echo $player1->getGp();?></td>
              <td><?php echo $player1->getPpg();?></td>
              <td><?php echo $player1->getReb();?></td>
              <td><?php echo $player1->getAst();?></td>
            </tr> -->
            <?php
            
        }
        ?>
        
          </table>
          <?php
    } catch(PDOException $e) {
        echo 'ERROR: ' . $e->getMessage();
    }

}



?>
