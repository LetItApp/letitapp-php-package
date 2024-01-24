<?php

include_once './LetItApp.php';

// Example Usage
$apiKey = 'API KEY';
$letItAppAPI = new LetItAppAPI($apiKey);

// Creating a notification instance and setting its properties
$notification = (new Notification())
    ->setHeader('Notification Header')
    ->setDescription('Notification Description')
    ->setUrl('https://example.com')
    ->setTime('12:00:00')
    ->setDate('2024-01-28')
    ->setLang('cs')
    ->setId(false);

// Sending the notification
$response = $letItAppAPI->createOrUpdateNotification($notification);
echo "Response for creating/updating a notification:\n";
var_dump($response);

// Getting error information if an error occurred
if ($letItAppAPI->getResponseStatus($response) !== 201 && $letItAppAPI->getResponseStatus($response) !== 202) {
    $errorMessage = $letItAppAPI->getErrorMessage($response);
    echo "Error while creating or updating a notification: $errorMessage\n";
}

// Retrieving the list of notifications
$responseList = $letItAppAPI->getList();
echo "\nResponse for retrieving the list of notifications:\n";
var_dump($responseList);

// Retrieving the details of a notification
$notificationId = 1; // Replace with the actual notification ID
$responseDetail = $letItAppAPI->getNotification($notificationId);
echo "\nResponse for retrieving the details of the notification with ID $notificationId:\n";
var_dump($responseDetail);

// Deleting a notification
$responseDelete = $letItAppAPI->deleteNotification($notificationId);
echo "\nResponse for deleting the notification with ID $notificationId:\n";
var_dump($responseDelete);

// Sending a notification
$responseSend = $letItAppAPI->sendNotification($notificationId);
echo "\nResponse for sending the notification with ID $notificationId:\n";
var_dump($responseSend);
