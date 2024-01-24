<?php


/**
 * PHP knihovna pro přístup k API
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
     * Odesílá HTTP požadavek na API.
     *
     * @param string $method HTTP metoda (GET, POST, PUT, DELETE)
     * @param string $endpoint API koncový bod
     * @param array $data Data pro POST nebo PUT požadavky
     * @return array Odpověď API včetně statusu a dat
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
     * Získá seznam notifikací.
     *
     * @return array Odpověď API včetně statusu a dat
     */
    public function createOrUpdateNotification(Notification $notification): array {
        $data = $notification->getData();
        return $this->sendRequest('PUT', 'update-or-create', $data);
    }

     /**
     * Získá detail notifikace podle ID.
     *
     * @param int $id ID notifikace
     * @return array Odpověď API včetně statusu a dat
     */
    public function deleteNotification(int $id): array {
        return $this->sendRequest('DELETE', 'delete/' . $id);
    }

    /**
     * Vytvoří nebo aktualizuje notifikaci.
     *
     * @param array $data Data pro vytvoření nebo aktualizaci notifikace
     * @return array Odpověď API včetně statusu a dat
     */
    public function getNotification(int $id): array {
        return $this->sendRequest('GET', 'show/' . $id);
    }

    /**
     * Smaže notifikaci podle ID.
     *
     * @param int $id ID notifikace
     * @return array Odpověď API včetně statusu a dat
     */
    public function getList(): array {
        return $this->sendRequest('GET', 'list');
    }

    /**
     * Odešle notifikaci okamžitě.
     *
     * @param int $id ID notifikace
     * @return array Odpověď API včetně statusu a dat
     */
    public function sendNotification(int $id): array {
        return $this->sendRequest('POST', 'send/' . $id);
    }

    /**
     * Získá informace o chybě v odpovědi API.
     *
     * @param array $response Odpověď API
     * @return string|null Text chyby nebo null, pokud chyba není k dispozici
     */
    public function getErrorMessage(array $response): ?string {
        return $response['response']['error'] ?? null;
    }

    /**
     * Získá status odpovědi API.
     *
     * @param array $response Odpověď API
     * @return int Status odpovědi
     */
    public function getResponseStatus(array $response): int {
        return $response['status'] ?? 0;
    }
}
