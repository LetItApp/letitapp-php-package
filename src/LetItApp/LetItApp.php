<?php


/**
 * PHP library for accessing the API
 *
 * @author Daniel Papik <app@letitapp.com>
 * @version 1.0
 */
class LetItAppAPI {

    private $apiKey;
    private $apiUrl;

    public function __construct(string $apiKey) {
        $this->apiKey = $apiKey;
        $this->apiUrl = 'https://api.letitapp.com/api/v1/';
    }

     /**
     * Sends an HTTP request to the API.
     *
     * @param string $method HTTP method (GET, POST, PUT, DELETE)
     * @param string $endpoint API endpoint
     * @param array $data Data for POST or PUT requests
     * @return array API response including status and data
     */
    private function sendRequest(string $method, string $endpoint, array $data = []): array {
        $url = $this->apiUrl . $endpoint;

        $headers = [
            'X-API-Key: ' . $this->apiKey,
            'Content-Type: application/json',
        ];

        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, $method);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);

        if ($method === 'POST' || $method === 'PUT') {
            curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($data));
        }

        $response = curl_exec($ch);
        $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        curl_close($ch);

        return [
            'status' => $httpCode,
            'response' => json_decode($response, true),
        ];
    }

      /**
     * Creates or updates a notification.
     *
     * @return array API response including status and data
     */
    public function createOrUpdateNotification(Notification $notification): array {
        $data = $notification->getData();
        return $this->sendRequest('PUT', 'update-or-create', $data);
    }

     /**
     * Deletes a notification
     *
     * @param int $id Notification ID
     * @return array API response including status and data
     */
    public function deleteNotification(int $id): array {
        return $this->sendRequest('DELETE', 'delete/' . $id);
    }

    /**
     * Gets the details of a notification by ID.
     *
     * @param array $data Data for creating or updating a notification
     * @return array API response including status and data
     */
    public function getNotification(int $id): array {
        return $this->sendRequest('GET', 'show/' . $id);
    }

    /**
     * Gets a list of notifications.
     *
     * @param int $id Notification ID
     * @return array API response including status and data
     */
    public function getList(): array {
        return $this->sendRequest('GET', 'list');
    }

    /**
     * Sends a notification immediately.
     *
     * @param int $id Notification ID
     * @return array API response including status and data
     */
    public function sendNotification(int $id): array {
        return $this->sendRequest('POST', 'send/' . $id);
    }

    /**
     * Gets information about an error in the API response.
     *
     * @param array $response API response
     * @return string|null Error message or null if error is not available
     */
    public function getErrorMessage(array $response): ?string {
        return $response['response']['error'] ?? null;
    }

    /**
     * Gets the status of the API response.
     *
     * @param array $response API response
     * @return int Response status
     */
    public function getResponseStatus(array $response): int {
        return $response['status'] ?? 0;
    }
}
