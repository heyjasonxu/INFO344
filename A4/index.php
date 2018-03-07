<?php
header('Content-Type: application/json');
$result = array("result" => "none");
require("player.php");

if(isset($_GET['name'])) {
    $term = $_GET["name"];

    try {
        $conn = new PDO('mysql:host=info344db.cchp6b1kx7uy.us-west-2.rds.amazonaws.com;dbname=info344db', 'heyjasonxu', 'Xuweiming34');
        $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);    
        
        $stmt = $conn->prepare("SELECT * FROM `15-16nba_stats` WHERE Name LIKE '%$term%'");
        $stmt->execute();
        
        // echo json_encode(array("answer" => "get"));
        // $one = count($stmt) == 1;
        //echo $stmt->rowCount();
        // echo $one;
        if($stmt->rowCount() == 1){
            while($row = $stmt->fetch()) {
                $player1 = new Player($row);
                $player_name = $player1->getName();
                echo $_GET['callback']. '(' . json_encode($player1 -> playerJson()) . ');';
                break;
            }
        } else {
            echo $_GET['callback']. '(' . json_encode($result) . ');';
        }
        
    } catch(PDOException $e) {
        echo $_GET['callback']. '(' . json_encode($e) . ');';
    }

} else {
    echo json_encode($result);
}
?>
