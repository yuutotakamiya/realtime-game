<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Mail extends Model
{
    use HasFactory;

    protected $guarded = [
        'id',
    ];

    public function mails()
    {
        return $this->belongsToMany(User::class, 'user_mails', 'mail_id', 'user_id');
    }
}
