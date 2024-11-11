<?php

namespace Database\Seeders;

use App\Models\stageLog;
use Illuminate\Database\Seeder;

class StageLogTableSeeder extends Seeder
{
    public function run(): void
    {
        stageLog::create([
            'user_id' => 1,
            'stage_id' => 1,
            'result' => 0,
            'min_hand_num' => 3
        ]);
    }
}
