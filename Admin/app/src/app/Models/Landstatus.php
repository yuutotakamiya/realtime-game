<?php

namespace App\Models;

use http\Encoding\Stream\Deflate;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Landstatus extends Model
{
    use HasFactory;

    protected $guarded = [
        'id',
    ];

    public function land_status()
    {
        return $this->hasMany(landStatus::class);
    }
}
