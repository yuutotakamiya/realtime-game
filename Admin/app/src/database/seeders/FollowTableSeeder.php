<?php

namespace Database\Seeders;

use App\Models\Follow;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class FollowTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        //
        Follow::create([
            'user_id' => 1,
            'follow_user_id' => 3,
        ]);
        Follow::create([
            'user_id' => 2,
            'follow_user_id' => 3,
        ]);
        Follow::create([
            'user_id' => 3,
            'follow_user_id' => 2,
        ]);
        Follow::create([
            'user_id' => 1,
            'follow_user_id' => 2,
        ]);
    }
}
