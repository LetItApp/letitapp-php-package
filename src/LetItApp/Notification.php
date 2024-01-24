<?php

/**
 * Model for LetItApp notification
 *
 * @author Daniel Papik <app@letitapp.com>
 * @version 1.0
 */
class Notification {
    private $header;
    private $description;
    private $url;
    private $time;
    private $date;
    private $lang;
    private $id;

    /**
     * Set the header of the notification.
     *
     * @param string $header The subject of the notification.
     * @return Notification The current instance for method chaining.
     */
    public function setHeader(string $header) {
        $this->header = $header;
        return $this;
    }

    /**
     * Set the description of the notification.
     *
     * @param string $description The content or description of the notification.
     * @return Notification The current instance for method chaining.
     */
    public function setDescription(string $description) {
        $this->description = $description;
        return $this;
    }

    /**
     * Set the URL associated with the notification.
     *
     * @param string $url The redirect link when the user clicks on the notification.
     * @return Notification The current instance for method chaining.
     */
    public function setUrl(string $url) {
        $this->url = $url;
        return $this;
    }

    /**
     * Set the time of the notification dispatch.
     *
     * @param string $time The time of the notification dispatch.
     * @return Notification The current instance for method chaining.
     */
    public function setTime(string $time) {
        $this->time = $time;
        return $this;
    }

    /**
     * Set the date of the notification dispatch.
     *
     * @param string $date The date of the notification dispatch.
     * @return Notification The current instance for method chaining.
     */
    public function setDate(string $date) {
        $this->date = $date;
        return $this;
    }

    /**
     * Set the language of the notification.
     *
     * @param string $lang The language of the notification.
     * @return Notification The current instance for method chaining.
     */
    public function setLang(string $lang) {
        $this->lang = $lang;
        return $this;
    }

    /**
     * Set the unique identifier for the notification.
     *
     * @param int $id The unique identifier for the notification.
     * @return Notification The current instance for method chaining.
     */
    public function setId(int $id) {
        $this->id = $id;
        return $this;
    }

    /**
     * Get the data of the notification as an associative array.
     *
     * @return array The data of the notification.
     */
    public function getData(): array {
        return [
            'header' => $this->header,
            'description' => $this->description,
            'url' => $this->url,
            'time' => $this->time,
            'date' => $this->date,
            'lang' => $this->lang,
            'id' => $this->id,
        ];
    }
}
