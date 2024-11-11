<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class UserMail extends Model
{
    protected $guarded = [
        'id',
    ];

    public function mails()
    {
        return $this->hasMany(User::class)->with('condition');
    }


}
