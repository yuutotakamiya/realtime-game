<?php

namespace Database\Seeders;

use App\Models\land;
use Illuminate\Database\Seeder;

class LandsTableSeeder extends Seeder
{
    public function run(): void
    {
        Land::create([
            'stage_id' => 1,
            'block_mission_sum' => 100,
            'result' => 0
        ]);
    }
}
