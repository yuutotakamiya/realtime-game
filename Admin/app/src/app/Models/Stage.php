<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Stage extends Model
{

    public function stage_logs()
    {
        return $this->hasMany(StageLog::class);
    }
}
