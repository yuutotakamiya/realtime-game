<?php

namespace Database\Seeders;

use App\Models\Item;
use App\Models\Player;
use App\Models\User;
use Database\Factories\UserFactory;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class UsersTableSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {

        //
        User::create([
            'name' => '勇者',
            'token'=>'',
            'point'=>0
        ]);
        User::create([
            'name' => 'aa',
            'token'=>'',
             'point'=>1
        ]);
        User::create([
            'name' => 'takamiya',
            'token'=>'',
            'point'=>2
        ]);
        User::create([
            'name' => 'uuu',
            'token'=>'',
            'point'=>3
        ]);
    }
}
